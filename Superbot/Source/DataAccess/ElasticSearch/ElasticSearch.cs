/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Model = Infosys.Solutions.Ainauto.Resource.DataAccess.ElasticSearch.Model;


namespace Infosys.Solutions.Ainauto.Resource.DataAccess.ElasticSearch
{
    public class ElasticSearch
    {
        ElasticClient client;
        public ElasticSearch()
        {
            var node = new Uri(System.Configuration.ConfigurationManager.AppSettings["ESBaseURL"]);
            var settings = new ConnectionSettings(node);
            client = new ElasticClient(settings);
        }

        public bool Insert(Model.ElasticSearchInput inputObj, string indexName)
        {
            if (!client.Indices.Exists(indexName).Exists)
            {
                var res = client.Indices.Create(indexName);                

                if (!res.IsValid)
                {
                    // if the index creation fails, then we return false
                    return false;
                }

            }
            var result = client.Index(inputObj, i => i.Index(indexName));
            if (!result.IsValid)
            {
                // if the index insertion fails, then we return false
                return false;
            }

            //all went well, we return true
            return true;
        }

        public bool InsertWithManualMapping(Model.ElasticSearchInputTest inputObj, string indexName)
        {
            if (!client.Indices.Exists(indexName).Exists)
            {
                var res = client.Indices.Create(indexName);

                //var res = client.Indices.Create(indexName, c => c
                // .Map<Model.ElasticSearchInputTest>(m => m.AutoMap()
                // .Properties(ps => ps
                //                .Date(d => d
                //                        .Name(n => n.IncidentCreateTime)
                //                        .Format("yyyy/MM/dd"))
                //                .Date(d => d
                //                        .Name(n => n.MetricTime)
                //                        .Format("yyyy/MM/dd"))
                // )
                // ));

                if (!res.IsValid)
                {
                    // if the index creation fails, then we return false
                    return false;
                }

            }
            var result = client.Index(inputObj, i => i.Index(indexName));
            if (!result.IsValid)
            {
                // if the index insertion fails, then we return false
                return false;
            }

            //all went well, we return true
            return true;
        }

        public List<Model.ElasticSearchInput> GetDocx(string indexName,string input)
        {
            List<Model.ElasticSearchInput> result = new List<Model.ElasticSearchInput>();
            var res = client.Search<Model.ElasticSearchInput>(s=>s.Index(indexName)
            .Query(q=>q
                .Match(m=>m
                    .Field(f=>f
                        .ResourceId)
                        .Query(input))));

            if (res.IsValid && res.Documents.ToList().Count!=0)
            {
                result = res.Documents.ToList();
            }
            return result;
        }

        public bool DeleteIndexDocuments(string indexName)
        {
            bool status = true;
            if (client.Indices.Exists(indexName).Exists)
            {
                var res = client.DeleteByQuery<Model.ElasticSearchInput>(D=>D.Index(indexName).Query(q => q.QueryString(qs => qs.Query("*"))));

                if (!res.IsValid)
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
            return status;
        }

        public bool DeleteIndex(string indexName)
        {
            bool status = true;
            if (client.Indices.Exists(indexName).Exists)
            {
                var res = client.Indices.Delete(indexName);
                if (!res.IsValid)
                {
                    status = false;
                }
            }
            else
            {
                status = false;
            }
            return status;
        }
    }
}
