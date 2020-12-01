module GoogleApiRequest 
open System.Net.Http
open System.Net.Http.Headers

let token = "ya29.a0AfH6SMBACS4bF6KAGzOzRFkjO1DiLPkDrybPmfMcSKBO7LBzDH_j-sJz-T7AI5HFui6hz8Fwy64VbRF88bz7-sz3YhastiitO9HwG_B32oAtGF3yvv--SVWfi2car87kxgBVmXQYhZxe5JldJ_p7exXZn-wpsa_En-sCEkpe9Lw"

let readContent (r: HttpResponseMessage) =
    r.Content.ReadAsStringAsync() |> Async.AwaitTask

let getClientWithAuthorization() = 
    let client = new HttpClient()
    client.DefaultRequestHeaders.Authorization <- new AuthenticationHeaderValue("Bearer", token)
    client

let authorizedGetRequest(requestUri: string) =
    async {
        let client = getClientWithAuthorization()
        let! response = requestUri |> client.GetAsync |> Async.AwaitTask
        let! result = response |> readContent
        return result
    }

let authorizedPostRequest (requestUri: string) (content: HttpContent) =
    async {
        let client = getClientWithAuthorization()
        let! response = (requestUri, content) |> client.PostAsync |> Async.AwaitTask
        let! result = response |> readContent
        return result
    }
    