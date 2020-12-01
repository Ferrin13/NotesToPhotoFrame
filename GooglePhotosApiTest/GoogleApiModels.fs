module GoogleApiModels

module Photos =
    type SimpleMediaItem = { fileName: string; uploadToken: string }
    type NewMediaItem = { description: string; simpleMediaItem: SimpleMediaItem }
    type MediaItemBatchCreateRequest = { albumId: string; newMediaItems: List<NewMediaItem> }

    type Album = { title: string }
    type AlbumCreateRequest = { album: Album }

module Auth = 
    type RefreshTokenResponse = { accessToken: string; expiresIn: int; }