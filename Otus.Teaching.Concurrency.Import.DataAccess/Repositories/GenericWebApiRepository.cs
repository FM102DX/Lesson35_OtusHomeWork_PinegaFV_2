using System;
using Otus.Teaching.Concurrency.Import.Core.Entities;
using Otus.Teaching.Concurrency.Import.Core.Repositories;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Otus.Teaching.Concurrency.Import.Core.Service;
using Otus.Teaching.Concurrency.Import.DataAccess.Data;
using System.Threading.Tasks;
using System.Linq;
using Otus.Teaching.Concurrency.Import.Core.Abstract;
using Otus.Teaching.Concurrency.Import.Core.Service;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Otus.Teaching.Concurrency.Import.DataAccess.Repositories
{
    public class GenericWebApiRepository<T> : IGenericRepository<T> where T : KeepableClass, IKeepable
    {
        //GenericRepository на webApi

        
        private static readonly object _locker = new object();

        private HttpClient httpClient;
        string _getAllPrefix;
        string _addItemPrefix;
        string _updateItemPrefix;
        string _deleteItemPrefix;
        bool _getPostOnlyMode; //если true, то репозиторий делает операции put и delete через post, т.к. у хостера put и delete выдают 405

        public string Guid {get; private set;}
        public string DbContextGuid { get { return ""; } }

        public GenericWebApiRepository(string baseAddress,
                                            bool getPostOnlyMode = true,
                                            string getAllPrefix="GetAll/",
                                            string addItemPrefix="",
                                            string updateItemPrefix="",
                                            string deleteItemPrefix="")
        {
            Guid = ServiceFunctions.generateNBlockGUID(1, 4);
            httpClient = new System.Net.Http.HttpClient(new HttpClientHandler());
            httpClient.BaseAddress = new Uri(baseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _getAllPrefix = getAllPrefix;
            _addItemPrefix = addItemPrefix;
            _updateItemPrefix = updateItemPrefix;
            _deleteItemPrefix = deleteItemPrefix;
            _getPostOnlyMode = getPostOnlyMode;

        }

        public long Count 
        { 
            get 
            { 
                return -1;
            } 
        }

        public IEnumerable<T> GetAllItems()
        {
            try
            {
                Task<HttpResponseMessage> response;
                string json;
                response = httpClient.GetAsync(_getAllPrefix);
                json = response.Result.Content.ReadAsStringAsync().Result;
                var items = JsonConvert.DeserializeObject<List<T>>(json);

                return items;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebApiRepository error: {ex.Message}");
            }
            return null;
        }

        public T GetItemByIdOrNull(int id)
        {
            try
            {
                Task<HttpResponseMessage> response;

                string json;

                response = httpClient.GetAsync($"{id}");

                //тут возвращается not found 404
                switch (response.Result.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        json = response.Result.Content.ReadAsStringAsync().Result;
                        var item = JsonConvert.DeserializeObject<T>(json);
                        return item;

                    case System.Net.HttpStatusCode.NotFound:
                        return null;

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebApiRepository error: {ex.Message}");
            }
            return null;
        }

        public bool Exists (int id)
        {
            T targetObejct = GetItemByIdOrNull(id);
            return targetObejct == null;

        }

        public CommonOperationResult AddItem(T t)
        {
            try
            {
                Task<HttpResponseMessage> response;
                string json;
                StringContent jsonContent;

                json = JsonConvert.SerializeObject(t, Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                response = httpClient.PostAsync($"", jsonContent);

                switch (response.Result.StatusCode)
                {
                    default:
                    case System.Net.HttpStatusCode.OK:
                        return CommonOperationResult.sayOk(response.Result.Content.ReadAsStringAsync().Result);

                    case System.Net.HttpStatusCode.Conflict:
                        return CommonOperationResult.sayFail(response.Result.Content.ReadAsStringAsync().Result);
                }
                // Console.WriteLine($"Server reply: code={response.Result.StatusCode} rezult={}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebApiRepository error: {ex.Message}");
            }
            return null;
        }

        public CommonOperationResult UpdateItem(T t)
        {
           
            if (_getPostOnlyMode)
            {
                    try
                    {
                        Task<HttpResponseMessage> response;
                        string json;
                        StringContent jsonContent;

                        json = JsonConvert.SerializeObject(t, Formatting.Indented,
                                new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                        jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                        response = httpClient.PostAsync($"update/", jsonContent);

                        switch (response.Result.StatusCode)
                        {
                            default:
                            case System.Net.HttpStatusCode.OK:
                                return CommonOperationResult.sayOk(response.Result.Content.ReadAsStringAsync().Result);

                            case System.Net.HttpStatusCode.Conflict:
                                return CommonOperationResult.sayFail(response.Result.Content.ReadAsStringAsync().Result);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"WebApiRepository error: {ex.Message}");
                    }
            }
            else
            {
                try
                {
                    Task<HttpResponseMessage> response;
                    string json;
                    StringContent jsonContent;

                    json = JsonConvert.SerializeObject(t, Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                    response = httpClient.PutAsync($"", jsonContent);

                    switch (response.Result.StatusCode)
                    {
                        default:
                        case System.Net.HttpStatusCode.OK:
                            return CommonOperationResult.sayOk(response.Result.Content.ReadAsStringAsync().Result);

                        case System.Net.HttpStatusCode.Conflict:
                            return CommonOperationResult.sayFail(response.Result.Content.ReadAsStringAsync().Result);
                    }
                    // Console.WriteLine($"Server reply: code={response.Result.StatusCode} rezult={}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebApiRepository error: {ex.Message}");
                }
            }
                return null;

            }

        public CommonOperationResult Delete(int id)
        {
            if (_getPostOnlyMode)
            {
                try
                {
                    Task<HttpResponseMessage> response;
                    string json;
                    StringContent jsonContent;

                    json = JsonConvert.SerializeObject(id, Formatting.Indented,
                            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                    jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

                    response = httpClient.PostAsync($"delete/{id}", jsonContent);

                    switch (response.Result.StatusCode)
                    {
                        default:
                        case System.Net.HttpStatusCode.OK:
                            return CommonOperationResult.sayOk(response.Result.Content.ReadAsStringAsync().Result);

                        case System.Net.HttpStatusCode.Conflict:
                            return CommonOperationResult.sayFail(response.Result.Content.ReadAsStringAsync().Result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebApiRepository error: {ex.Message}");
                }
            }
            else
            {
                try
                {
                    Task<HttpResponseMessage> response;
                    response = httpClient.DeleteAsync($"{id}");

                    switch (response.Result.StatusCode)
                    {
                        default:
                        case System.Net.HttpStatusCode.OK:
                            return CommonOperationResult.sayOk(response.Result.Content.ReadAsStringAsync().Result);
                        case System.Net.HttpStatusCode.Conflict:
                            return CommonOperationResult.sayFail(response.Result.Content.ReadAsStringAsync().Result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"WebApiRepository error: {ex.Message}");
                }
            }

            return null;
        }

        public void PrintItemListToConsole()
        {
            IEnumerable<T> z = GetAllItems();
            int counter = 0;
            foreach (var x in z)
            {
                Console.WriteLine(x.ToString());
                counter++;
            }
        }

        public void PrintItemListToFile(string path)
        {
            FileWriter file = new FileWriter(path);
            int counter = 0;

            IEnumerable<T> z = GetAllItems();
            foreach (var x in z)
            {
                file.DoWrite(x.ToString());
                counter++;
            }
            file.DoWrite($"Обработано {counter} записей");
            file.Close();
        }

        public void PrepareDb()
        {

        }

        /*
        public IEnumerable<T> GetItemsFiltered(Func<T, bool> predicate)
        {
            return null;
            //return _dbSet.AsNoTracking(). Where(predicate).ToList();
        }

        */
    }
}