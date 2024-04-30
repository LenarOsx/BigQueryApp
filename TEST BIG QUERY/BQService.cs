using Google.Cloud.BigQuery.V2;

namespace TEST_BIG_QUERY
{
    public class BQService
    {
        private string projectId = "operation-datalake";

        public BQService()
        {

        }
        public List<T> Execute<T>(string datasetId,string tableId,int numToTake)
        {
            var client = BigQueryClient.Create(projectId);

            List<T> result = new List<T>();
            try
            {
                //var table = client.GetTable("operation-datalake", "Naturgy", "Naturgy_Inin_Historico");
                var table = client.GetTable(projectId, datasetId, tableId);
                string query = $"SELECT *,TIMESTAMP(CONCAT(Fecha,\" \",REPLACE(Hora,\" \",\"\"))) AS StartDate FROM {table} ORDER BY StartDate LIMIT 10"; ;
                BigQueryResults results = client.ExecuteQuery(query, parameters: null);
                List<string> fields = new List<string>();

                foreach (var col in results.Schema.Fields)
                {
                    fields.Add(col.Name);
                }

                Dictionary<string, object> rowoDict;

                foreach (var row in results)
                {
                    rowoDict = new Dictionary<string, object>();
                    foreach (var col in fields)
                    {
                        rowoDict.Add(col, row[col]);
                    }
                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(rowoDict);
                    T o = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
                    result.Add(o);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                client.Dispose();
                Console.WriteLine("Done.");
            }

            return result;

        }

    }
}
