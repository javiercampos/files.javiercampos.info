// Jcl: simple docker healthcheck tool
// Created to remove the "curl" dependency on alpine
var http = new HttpClient();
try
{
    var result = await http.GetAsync("http://localhost:80/healthcheck");
    result.EnsureSuccessStatusCode();
}
catch
{
    return 1;
}

return 0;
