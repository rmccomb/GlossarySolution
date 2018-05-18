using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Linq;
using System.Threading.Tasks;

namespace Glossary.Tests.External
{
    #region Client-side JSON types

    public class Definition
    {
        public string Term;
        public string TermDefinition;
    }


    public class DefinitionResponse
    {
        public int RecordsUpdated;
        public int RecordsTotal;
    }

    #endregion

    /// <summary>
    /// External API tests
    /// </summary>
    [TestClass]
    public class ExternalTests
    {
        static readonly Uri uri = new Uri("http://localhost:59650/api/");

        private void GivenEmptyDataset()
        {
            using (var ctx = new Data.GlossaryEntities())
            {
                if (ctx.Definitions.Any())
                {
                    var defs = from d in ctx.Definitions select d;
                    ctx.Definitions.RemoveRange(defs);
                    ctx.SaveChanges();
                }
            }
        }

        public Definition[] GivenTerms()
        {
            return new Definition[]
            {
                new Definition { Term = "abyssal plain", TermDefinition = "The ocean floor off the continental margin, usually very flat with a slight slope." },
                new Definition { Term="accrete", TermDefinition="To add terranes (small land masses or pieces of crust) to another, usually larger, land mass." },
                new Definition { Term="alkaline", TermDefinition = "Term pertaining to highly basic, as opposed to acidic, substance" }
            };
        }

        [TestMethod]
        public void GetExistingTerms()
        {
            var resp = MakeUrlGetRequest("Definitions");
            //Debug.WriteLine(resp);
            var definitions = new JavaScriptSerializer().Deserialize<Definition[]>(resp);
            Assert.IsNotNull(definitions);
            definitions.ToList().ForEach(d => Debug.WriteLine($"{d.Term} {d.TermDefinition}"));
        }

        [TestMethod]
        public void PostNewTerms()
        {
            GivenEmptyDataset();
            var terms = GivenTerms();

            var json = new JavaScriptSerializer().Serialize(terms);
            var resp = MakeHttpPostRequest("Definitions", json);

            Debug.WriteLine(resp);
        }

        [TestMethod]
        public void PostPrexistingTerms()
        {
            GivenEmptyDataset();

            var terms = GivenTerms();
            PostNewTerms();

            terms.First().TermDefinition = "=== Updated definition ===";

            var json = new JavaScriptSerializer().Serialize(terms);
            var resp = MakeHttpPostRequest("Definitions", json);

            Debug.WriteLine(resp);

            var defResp = new JavaScriptSerializer().Deserialize<DefinitionResponse>(resp);
            Assert.IsTrue(defResp.RecordsUpdated == 1);
            Assert.IsTrue(defResp.RecordsTotal == 3);
        }

        [TestMethod]
        public async Task PostTermsErrorAsync()
        {
            // Validation of too short term
            var terms = new Definition[] { new Definition { Term = "A", TermDefinition = "B" } };
            var json = new JavaScriptSerializer().Serialize(terms);
            Debug.WriteLine(json);

            var resp = await MakeHttpPostRequestAsync("Definitions", json);

            Debug.WriteLine(resp.StatusCode);
            Assert.IsTrue(resp.StatusCode == System.Net.HttpStatusCode.BadRequest);
        }

        static string MakeHttpPostRequest(string command, object content)
        {
            var client = GetHttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var subCmd = new Uri(uri + command);
            Debug.WriteLine(subCmd);
            var response = client.PostAsJsonAsync(subCmd, content);
            response.Result.EnsureSuccessStatusCode();
            return response.Result.Content.ReadAsStringAsync().Result;
        }

        static async Task<HttpResponseMessage> MakeHttpPostRequestAsync(string command, object content)
        {
            var client = GetHttpClient();
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            var subCmd = new Uri(uri + command);
            Debug.WriteLine(subCmd);
            return await client.PostAsJsonAsync(subCmd, content);
        }

        static HttpClient GetHttpClient()
        {
            var client = new HttpClient { BaseAddress = uri };
            //client.DefaultRequestHeaders.TryAddWithoutValidation("authorization", "Bearer " + Token);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
            return client;
        }

        static string MakeUrlGetRequest(string command, string parameters = null)
        {
            using (var client = GetHttpClient())
            {
                var cmd = $"{command}?{parameters}";
                Debug.WriteLine(cmd);
                using (var response = client.GetAsync(cmd).Result)
                {
                    Debug.WriteLine(response.StatusCode);
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
