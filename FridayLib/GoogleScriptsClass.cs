using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace FridayLib
{
    /// <summary>
    /// Логика работы с таблицей Google
    /// </summary>
    public class GoogleScriptsClass
    {
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string AppName = "Friday";

        public static void UpdateSheetsData(IEnumerable<CFile> files)
        {
            try
            {
                var credential = GetCredintials();
                var service = GetService(credential);
                String spreadsheetId = "10dymgee_7SNKLRwf9nS533pJpTMk1tLbndR9BmdO8As";
                String range = "Актуальные версии!A2:N100";
                int sheetId = 1539764994;
                //int sheetId = 1515691245;
                SpreadsheetsResource.ValuesResource.GetRequest dataRequest =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = dataRequest.Execute();
                IList<IList<Object>> exValues = response.Values;

                List<Request> requests = new List<Request>();

                foreach (var file in files)
                {
                    foreach (var val in exValues)
                    {

                        if (file.ProjectName == val[1].ToString() & !string.IsNullOrEmpty(file.SourcePath))
                        {
                            List<CellData> values = new List<CellData>()
                        {
                            new CellData(){UserEnteredValue = new ExtendedValue { NumberValue = file.ID }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.ProjectName }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.Name }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.SourceVersion }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = val[4].ToString() }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.SourceDate.Date.ToString("dd.MM.yyyy") }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.SourceHash }},
                        };

                            requests.Add(new Request()
                            {
                                UpdateCells = new UpdateCellsRequest()
                                {
                                    Start = new GridCoordinate()
                                    {
                                        SheetId = sheetId,
                                        ColumnIndex = 0,
                                        RowIndex = file.ID
                                    },
                                    Rows = new List<RowData>() { new RowData() { Values = values } },
                                    Fields = "userEnteredValue"
                                }
                            });
                            break;
                        }
                    }

                }
                BatchUpdateSpreadsheetRequest request = new BatchUpdateSpreadsheetRequest()
                {
                    Requests = requests
                };

                service.Spreadsheets.BatchUpdate(request, spreadsheetId).Execute();
            }
            catch(Exception ex)
            {
                
            }
            
        }

        public static List<CFile> GetSheetsData()
        {
            List<CFile> result = new List<CFile>();
            try
            {
                var credential = GetCredintials();
                var service = GetService(credential);
                String spreadsheetId = "10dymgee_7SNKLRwf9nS533pJpTMk1tLbndR9BmdO8As";
                String range = "Актуальные версии!A2:N100";
                SpreadsheetsResource.ValuesResource.GetRequest request =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = request.Execute();
                IList<IList<Object>> values = response.Values;

                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        result.Add(new CFile()
                        {
                            ID = Convert.ToInt32(row[0]),
                            ProjectName = row[1].ToString(),
                            Name = row[2].ToString(),
                            ReleasePath = "",
                            SourcePath = "",
                            SourceHash = row[6].ToString(),
                            SourceVersion = row[3].ToString(),
                            SourceDate = Convert.ToDateTime(row[5])
                        });
                    }
                }
                else
                {
                    Console.WriteLine("No data found.");
                }
            }
            catch
            {

            }
           
            return result;
        }

        public static UserCredential GetCredintials()
        {
            UserCredential credential;
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "Vanny",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            return credential;
        }

        public static SheetsService GetService(UserCredential credential)
        {
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });
            return service;
        }

        public static void AddDataToSheet(CFile file)
        {
            try
            {
                var credeintial = GetCredintials();
                var service = GetService(credeintial);
                String spreadsheetId = "10dymgee_7SNKLRwf9nS533pJpTMk1tLbndR9BmdO8As";
                int sheetId = 1539764994;
                List<Request> requests = new List<Request>();
                List<CellData> values = new List<CellData>()
            {
                new CellData(){UserEnteredValue = new ExtendedValue { NumberValue = file.ID }},
                new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.ProjectName }},
                new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.Name }},
                new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.SourceVersion }},
                new CellData(){UserEnteredValue = new ExtendedValue { StringValue = "" }},
                new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.SourceDate.Date.ToString("dd.MM.yyyy") }},
                new CellData(){UserEnteredValue = new ExtendedValue { StringValue = file.SourceHash }},
            };
                requests.Add(new Request()
                {
                    UpdateCells = new UpdateCellsRequest()
                    {
                        Start = new GridCoordinate()
                        {
                            SheetId = sheetId,
                            ColumnIndex = 0,
                            RowIndex = file.ID + 1
                        },
                        Rows = new List<RowData>() { new RowData() { Values = values } },
                        Fields = "userEnteredValue"
                    }
                });
                BatchUpdateSpreadsheetRequest request = new BatchUpdateSpreadsheetRequest()
                {
                    Requests = requests
                };
                service.Spreadsheets.BatchUpdate(request, spreadsheetId).Execute();
            }
            catch
            {

            }
            
        }
    }
}
