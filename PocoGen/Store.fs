module Store

open Xamarin.Forms
open Xamarin.Essentials
open Models
open System

let addConnectionString (connString:ConnectionString) =
    async {
        try
            SecureStorage.SetAsync(connString.Name , connString.Value)
            return StoreConnectionStringResult.Success
        with
        | :? Exception as ex ->
            return StoreConnectionStringResult.Error ex.Message
    }

let getConnectionString (connName:ConnectionName ) =
    SecureStorage.GetAsync(connName);

let clearAllData =
    SecureStorage.RemoveAll

let removeConnection (connName:ConnectionName ) =
    SecureStorage.Remove connName