module GoogleApiRequest 
open System.Net.Http
open System.Net.Http.Headers
open System.Collections.Generic
open FSharp.Json
open GoogleApiModels.Auth
open System

let private clientId = Environment.GetEnvironmentVariable("NOTES_TO_PHOTO_FRAME_CLIENT_ID");
let private clientSecret = Environment.GetEnvironmentVariable("NOTES_TO_PHOTO_FRAME_CLIENT_SECRET");
let private refreshToken = Environment.GetEnvironmentVariable("NOTES_TO_PHOTO_FRAME_REFRESH_TOKEN");
let private mimmumRefreshBufferSeconds = 65000.0;

let private client = new HttpClient()
let mutable private accessToken = Option<string>.None
let mutable private expiresTime = DateTime.MinValue

let private readContent (r: HttpResponseMessage) =
    r.Content.ReadAsStringAsync() |> Async.AwaitTask

let private getAndSetAccessToken() =
    async {
        
        let values =  [
            KeyValuePair.Create("client_id", clientId)
            KeyValuePair.Create("client_secret", clientSecret)
            KeyValuePair.Create("grant_type", "refresh_token")
            KeyValuePair.Create("refresh_token", refreshToken) ]
        let content = new FormUrlEncodedContent(values)
        let! response = ("https://oauth2.googleapis.com/token", content) |> client.PostAsync |> Async.AwaitTask
        let! content = response |> readContent
        let config = JsonConfig.create(jsonFieldNaming = Json.snakeCase)
        let response = Json.deserializeEx<RefreshTokenResponse> config content
        accessToken <- Option.Some(response.accessToken)
        expiresTime <- response.expiresIn |> float |> DateTime.Now.AddSeconds
        return response.accessToken
    }

let private getElseRefreshAccessToken() =
    async {
        return!
            match (accessToken, expiresTime) with
            | (token, et) when token.IsSome && et > DateTime.Now.AddSeconds mimmumRefreshBufferSeconds -> async.Return accessToken.Value
            | _ -> getAndSetAccessToken()
    }

let private getClientWithAuthorization() = 
    async {
        let! token = getElseRefreshAccessToken()
        client.DefaultRequestHeaders.Authorization <- new AuthenticationHeaderValue("Bearer", token)
        return client
    }
    
let authorizedGetRequest(requestUri: string) =
    async {
        let! client = getClientWithAuthorization()
        let! response = requestUri |> client.GetAsync |> Async.AwaitTask
        return! response |> readContent
    }
    
let authorizedPostRequest (requestUri: string) (content: HttpContent) =
    async {
        let! client = getClientWithAuthorization()
        let! response = (requestUri, content) |> client.PostAsync |> Async.AwaitTask
        return!  response |> readContent
    }