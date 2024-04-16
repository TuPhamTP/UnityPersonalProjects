using System.Collections.Generic;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class GoogleCredentialInfo
{
    public string type;
    public string project_id;
    public string private_key_id;
    public string private_key;
    public string client_email;
    public string client_id;
    public string auth_uri;
    public string token_uri;
    public string auth_provider_x509_cert_url;
    public string client_x509_cert_url;
    public string universe_domain;
}

public class GoogleSheetAPI : MonoBehaviour
{
    public Text TextDebug;
    public Text TextJsonSample;
    private string strPart, strFull;

    public string spreadsheetId = "1KvE-eyOXRtwzMFJF5ipbKHQGVyMap2wplwm6OZHQM4c"; // ID của Google Sheet
    public string range = "Sheet1!A2:C4"; // Range chứa dữ liệu, ví dụ: "Sheet1!A1:C3"
    //string jsonFilePath = Path.Combine(Application.streamingAssetsPath, "test1project-420406-bb9a626f6d58.json");
    private GoogleCredentialInfo credentialInfo;

    private void Start()
    {
        // Đọc dữ liệu từ Google Sheet
        credentialInfo = JsonConvert.DeserializeObject<GoogleCredentialInfo>(TextJsonSample.text);
        ReadDataFromGoogleSheet();
    }

    private void ReadDataFromGoogleSheet()
    {
        /*
        GoogleCredential credential;
        using (var stream = new System.IO.FileStream(jsonFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(new[] { SheetsService.Scope.Spreadsheets });
        }
        */
        GoogleCredential credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(credentialInfo))
            .CreateScoped(new[] { SheetsService.Scope.Spreadsheets });

        // Khởi tạo Sheets API service
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Your Application Name",
        });

        // Gửi yêu cầu đọc dữ liệu
        SpreadsheetsResource.ValuesResource.GetRequest request =
            service.Spreadsheets.Values.Get(spreadsheetId, range);

        ValueRange response = request.Execute();
        IList<IList<object>> values = response.Values;

        // Xử lý dữ liệu nhận được
        if (values != null && values.Count > 0)
        {
            foreach (var row in values)
            {
                // row[0] là Name, row[1] là Phone, row[2] là Email (tùy vào range bạn chọn)
                Debug.Log($"Name: {row[0]}, Phone: {row[1]}, Email: {row[2]}");
                strPart = $"Name: {row[0]}, Phone: {row[1]}, Email: {row[2]}";
                strFull += strPart + "\n";
                TextDebug.text = strFull;
            }
        }
        else
        {
            Debug.Log("No data found.");
        }
    }

    public void WriteDataToGoogleSheet()
    {
        /*
        GoogleCredential credential;
        using (var stream = new System.IO.FileStream(jsonFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream)
                .CreateScoped(new[] { SheetsService.Scope.Spreadsheets });
        }
        */
        GoogleCredential credential = GoogleCredential.FromJson(JsonConvert.SerializeObject(credentialInfo))
            .CreateScoped(new[] { SheetsService.Scope.Spreadsheets });

        // Khởi tạo Sheets API service
        var service = new SheetsService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Your Application Name",
        });

        // Dữ liệu bạn muốn ghi vào Google Sheet
        ValueRange body = new ValueRange();
        var values = new List<object>() { "Tú Lê", "'0389490468", "tule@gmail.com" };
        body.Values = new List<IList<object>> { values };

        // Gửi yêu cầu ghi dữ liệu
        SpreadsheetsResource.ValuesResource.AppendRequest request =
            service.Spreadsheets.Values.Append(body, spreadsheetId, range);
        request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

        AppendValuesResponse response = request.Execute();
        Debug.Log("Data appended to Google Sheet!");
    }
}
