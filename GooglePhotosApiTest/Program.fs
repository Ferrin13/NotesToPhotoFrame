// Learn more about F# at http://fsharp.org

open System
open System.Net.Http
open GoogleApiRequest
open SixLabors.ImageSharp;
open System.IO
open GoogleApiModels.Photos
open FSharp.Json

let createImageMediaItem(path: string) =
    use image = Image.Load path
    use memoryStream = new MemoryStream()
    image.SaveAsPng memoryStream
    let binaryData = memoryStream.ToArray()
    let requestData = new ByteArrayContent(binaryData)
    authorizedPostRequest "https://photoslibrary.googleapis.com/v1/uploads" requestData

let addItemToAlbum (uploadToken: string) =
    let simpleItem = { fileName = "Test File Name"; uploadToken = uploadToken }
    let newMediaItem = { description = "Test Description"; simpleMediaItem = simpleItem}
    let request = { albumId = "AHZfi99CcoPFQHWP1MjMDt1jyVgM1kKeQtGhxdpeFom8ZBIR2P9Nk_VtbxKtkkvHpVEw0RIFiTMy"; newMediaItems = [newMediaItem] }
    let requestContent = new StringContent(Json.serialize request)
    authorizedPostRequest "https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate" requestContent

let uploadImage(path: string) = 
    async {
        let! uploadToken = createImageMediaItem path
        return! addItemToAlbum uploadToken
    }

let getAlbums() =
    authorizedGetRequest "https://photoslibrary.googleapis.com/v1/albums"

let createAlbum() =
    let request = { album = { title = "Test Created Alubm" } }
    let requestContent = new StringContent(Json.serialize request)
    authorizedPostRequest "https://photoslibrary.googleapis.com/v1/albums" requestContent

let uploadPicturesTest() =
    [
        "C:\Users\CalebsLaptop\Pictures\BSULogo.png"
        "C:\Users\CalebsLaptop\Pictures\GimpExperiments\TestQuote1.png"
        "C:\Users\CalebsLaptop\Pictures\GimpExperiments\TestQuote2.png"
        "C:\Users\CalebsLaptop\Pictures\GimpExperiments\TestQuote3.png"
        "C:\Users\CalebsLaptop\Pictures\GimpExperiments\TestQuote4.png"]
    |> Seq.map uploadImage
    |> Async.Sequential
    |> Async.RunSynchronously
    |> printfn "Response %A"

let getAlbumsTest() = 
    getAlbums()
    |> Async.RunSynchronously
    |> printfn "Response %A"

[<EntryPoint>]
let main argv =
    printfn "Quote uploader"
    getAlbumsTest()
    Console.ReadLine() |> ignore
    getAlbumsTest()
    0



