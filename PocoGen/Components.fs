namespace PocoGen.Components

open Fabulous
open Fabulous.XamarinForms
open Xamarin.Forms
open PocoGen.Models
open PocoGen.Messages

module Components =
    let getDbName (db: DbItem): string =
        db.Name
    let toDbItemPickerOption (dbs: DbItem list): string list =
        dbs |> List.map (getDbName)

    let toLanguagePickerOption (languages: Language list): string list =
        languages |> List.map (fun l -> l.ToString())

    let toConStrPickerOptions (connectionStringItems: ConnectionStringItem list): string list =
        connectionStringItems |> List.map (fun c -> c.Id.ToString() + c.Name)

    let toTableCell (tables: Table list): ViewElement list =
        tables |> List.map (fun i -> View.TextCell i.Name)

    let formLabel text = View.Label(text = text)

    let formOutputLabel text =
        View.Label
            (text = text, backgroundColor = Color.Black, textColor = Color.White,
             verticalOptions = LayoutOptions.FillAndExpand, horizontalOptions = LayoutOptions.FillAndExpand)

    let formOutput text =
        View.ScrollView
            (verticalOptions = LayoutOptions.FillAndExpand, horizontalOptions = LayoutOptions.FillAndExpand,
             content =
                 View.StackLayout
                     (verticalOptions = LayoutOptions.FillAndExpand, horizontalOptions = LayoutOptions.FillAndExpand,
                      children = [ formOutputLabel text ]))

    let formEntry text textChanged =
        View.Entry(text = text, textChanged = (fun e -> e.NewTextValue |> textChanged), height = 30.)

    let formEditor text textChanged =
        View.Editor(text = text, textChanged = (fun e -> e.NewTextValue |> textChanged), height = 100.)

    let formMultiLineEditor text textChanged =
        View.Editor(text = text, textChanged = (fun e -> e.NewTextValue |> textChanged), height = 100.)

    let formButton text cmd isEnabled =
        View.Button
            (verticalOptions = LayoutOptions.Fill, horizontalOptions = LayoutOptions.FillAndExpand,
             isEnabled = isEnabled, text = sprintf text, command = cmd)

    let List col =
        View.ListView
            (items =
                [ for item in col do
                    yield View.TextCell(item) ])

    let verticalStack children =
        View.StackLayout
            (orientation = StackOrientation.Vertical, verticalOptions = LayoutOptions.FillAndExpand,
             horizontalOptions = LayoutOptions.FillAndExpand, padding = Thickness(20.0), margin = Thickness(20.0),
             children = children)

    let horizontalStack children =
        View.StackLayout
            (orientation = StackOrientation.Horizontal, verticalOptions = LayoutOptions.StartAndExpand,
             horizontalOptions = LayoutOptions.StartAndExpand, padding = Thickness(20.0), margin = Thickness(20.0),
             children = children)



    let connectionTestView (model: Model) dispatch =
        let isLoading = (model.CurrentFormState = Testing)
        let updateConnectionStringValue = UpdateConnectionStringValue >> dispatch
        let updateConnectionStringName = UpdateConnectionStringName >> dispatch
        let testConnection = (fun () -> dispatch (TestConnection))

        let testButton =
            (match model.CurrentFormState with
             | Valid -> formButton "Test" testConnection (not isLoading)
             | _ -> formButton "Test" testConnection false)

        let buttonStack =
            View.StackLayout
                (orientation = StackOrientation.Horizontal, verticalOptions = LayoutOptions.Center,
                 horizontalOptions = LayoutOptions.Fill, children = [ testButton ])


        verticalStack
            ([ View.ActivityIndicator(isVisible = isLoading, isRunning = isLoading)
               formLabel "Connection String Label"
               formEntry model.ConnectionString.Name updateConnectionStringName
               formLabel "Connection String Value"
               formMultiLineEditor model.ConnectionString.Value updateConnectionStringValue
               buttonStack ])

    let codeGenView (model: Model) dispatch =
        let dbItems: string list = model.Databases |> toDbItemPickerOption
        let langs: string list = model.Languages |> toLanguagePickerOption

        let conStrs: string list =
            model.ConnectionStrings |> toConStrPickerOptions

        let tables: ViewElement list = model.Tables |> toTableCell

        let innerLayout (children: ViewElement list): ViewElement =
            View.StackLayout
                (orientation = StackOrientation.Vertical, verticalOptions = LayoutOptions.StartAndExpand,
                 horizontalOptions = LayoutOptions.Start, margin = Thickness(1.0), children = children)

        let loadBtn: ViewElement =
            formButton "Load Db Connection Data" (fun () -> dispatch (LoadConnectionData)) true

        let buttonStack: ViewElement =
            View.StackLayout
                (orientation = StackOrientation.Horizontal, verticalOptions = LayoutOptions.Center,
                 horizontalOptions = LayoutOptions.Fill, children = [ loadBtn ])

        let body =
            innerLayout
                ([ buttonStack
                   formLabel "Connection Strings"
                   View.Picker(items = conStrs, title = "Connection Strings")
                   formLabel "Databases"
                   View.Picker(items = dbItems, title = "Databases")
                   formLabel "Languages"
                   View.Picker(items = langs, title = "Languages") ])

        let footer =
            innerLayout
                ([ formLabel "Code Gen"
                   View.ListView(items = tables) ])

        View.StackLayout
            (orientation = StackOrientation.Horizontal, verticalOptions = LayoutOptions.StartAndExpand,
             horizontalOptions = LayoutOptions.Start, padding = Thickness(1.0), margin = Thickness(20.0),
             children = [ body; footer ])
