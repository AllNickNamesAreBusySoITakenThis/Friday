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

        public static void UpdateSheetsData(IEnumerable<ControlledApp> applications)
        {
            try
            {
                var credential = GetCredintials();
                var service = GetService(credential);
                String spreadsheetId = ServiceLib.Configuration.Configuration.Get("SpreadsheetAddress").ToString();
                String range = "Test!A2:N100";
                int sheetId = Convert.ToInt32(ServiceLib.Configuration.Configuration.Get("SpreadsheetId"));
                SpreadsheetsResource.ValuesResource.GetRequest dataRequest =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = dataRequest.Execute();
                IList<IList<Object>> exValues = response.Values;

                List<Request> requests = new List<Request>();

                foreach (var app in applications)
                {
                    foreach (var val in exValues)
                    {

                        if (app.Name == val[1].ToString())
                        {
                            List<CellData> values = new List<CellData>()
                        {
                            new CellData(){UserEnteredValue = new ExtendedValue { NumberValue = Convert.ToInt32(val[0]) }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.Name }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.MainFileName }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.MainFileReleaseVersion }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = val.Count>=4?val[4].ToString():"АО \"НПО \"Спецэлектромеханика\"" }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = string.IsNullOrEmpty(app.MainFileReleaseDate)?"":app.MainFileReleaseDate.Remove(app.MainFileReleaseDate.IndexOf(" "))}},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.MainFileReleaseHash }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.Description }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = EnumHelper.Description(app.Parent.Category) }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.CompatibleOSs }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.CompatibleScadas }},
                            new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.CompatibleSZI }},
                        };

                            requests.Add(new Request()
                            {
                                UpdateCells = new UpdateCellsRequest()
                                {
                                    Start = new GridCoordinate()
                                    {
                                        SheetId = sheetId,
                                        ColumnIndex = 0,
                                        RowIndex = Convert.ToInt32(val[0])
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
            catch (Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при обновлении таблицы Google: {0}", ex.Message));
            }

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
        public static void AddDataToSheet(ControlledApp app)
        {
            try
            {
                var credeintial = GetCredintials();
                var service = GetService(credeintial);
                String spreadsheetId = ServiceLib.Configuration.Configuration.Get("SpreadsheetAddress").ToString();
                //int sheetId = 1539764994;
                int sheetId = Convert.ToInt32(ServiceLib.Configuration.Configuration.Get("SpreadsheetId"));
                String range = "Test!A2:N100";
                SpreadsheetsResource.ValuesResource.GetRequest dataRequest =
                        service.Spreadsheets.Values.Get(spreadsheetId, range);
                ValueRange response = dataRequest.Execute();
                IList<IList<Object>> exValues = response.Values;
                List<Request> requests = new List<Request>();
                List<CellData> values = new List<CellData>()
                {
                    new CellData(){UserEnteredValue = new ExtendedValue { NumberValue = exValues==null?1:exValues.Count+1 }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.Name }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.MainFileName }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.MainFileReleaseVersion }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = "АО \"НПО \"Спецэлектромеханика\"" }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = string.IsNullOrEmpty(app.MainFileReleaseDate)?"":app.MainFileReleaseDate.Remove(app.MainFileReleaseDate.IndexOf(" "))}},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.MainFileReleaseHash }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.Description }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = EnumHelper.Description(app.Parent.Category) }},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.CompatibleOSs}},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.CompatibleScadas}},
                    new CellData(){UserEnteredValue = new ExtendedValue { StringValue = app.CompatibleSZI}},
                };
                requests.Add(new Request()
                {
                    UpdateCells = new UpdateCellsRequest()
                    {
                        Start = new GridCoordinate()
                        {
                            SheetId = sheetId,
                            ColumnIndex = 0,
                            RowIndex = exValues == null ? 1 : exValues.Count + 1
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
            catch(Exception ex)
            {
                Service.OnErrorInLibrary(string.Format("Ошибка при добавлениии данных в таблицу Google: {0}", ex.Message));
            }

        }
    }
}
