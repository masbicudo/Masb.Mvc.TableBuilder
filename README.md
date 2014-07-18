Masb Mvc TableBuilder
=====================

Minimalist ASP.NET MVC Html Helper to help rendering tabular data, by creating table templates.

The real objective here is not to render any HTML.
This is the programmer's role.
There are so many helpers that render tables...

**[NuGet Package](https://www.nuget.org/packages/Masb.Mvc.TableBuilder)**

    PM> Install-Package Masb.Mvc.TableBuilder

It's all about separation
-------------------------

This tool is like a knive, in that it splits these aspects of a table:
- table contents
- table frame

This separation lets you create table components.

What this does not do
---------------------
- doesn't hide the table frame rendering from you... as most helpers already do
- doesn't hide table content rendering... most helpers also do this to some extent
- doesn't hide any HTML rendering from the programmer (or designer)
    - little exception: the hidden field for editing collections name like "Collection.Index"

Sample
------

This is a sample `TableTemplateFor`... it creates a template only. Nothing will be rendered yet.
You can place this in your cshtml view file:

```csharp
@{
    // creating a table template for the collection "Rows" of my model object
    var tableTemplate = this.Html
        .TableTemplateFor(m => m.Rows)
    
        // defining two column-level sections
        // these must be defined for each column
        .SetColumnSections("ColumnGroup", "HeaderHint")
        
        // adding property-specific columns
        // the submodel of this column will be the type of the property
        .AddColumnFor(m => m.PersonName,
            @<div>@item.Meta.GetDisplayName()</div>,    // defining header cells of the column
            @<div>@item.Html.EditorFor(m => m)</div>,   // defining the data cells of the column
            @<col style="width: 200px" />,              // defining "ColumnGroup" column-section
            @<text>Name of the person</text>)           // defining "HeaderHint" column-section
            
        .AddColumnFor(m => m.BirthDate,
            @<div>@item.Meta.GetDisplayName()</div>,
            @<div>@item.Html.EditorFor(m => m)</div>,
            null,                                       // you can set a column-section to null
            @<text>Date of birth</text>)                //     null means 'undefined'
            
        .AddColumnFor(m => m.Age,
            @<div>@item.Meta.GetDisplayName()</div>,
            @<div>@item.Html.EditorFor(m => m)</div>,
            null,
            @<text>Age of person</text>)
            
        // adding a column that is not property-specific
        // same as above, but without a property indication
        // the submodel of this column will be the type of the items in "Rows" collection
        .AddColumn(
            @<div>Male/Female property</div>,
            @<div>@(item.Model == null || item.Model.Gender == GenderKinds.Male
                                ? item.Html.EditorFor(m => m.MaleProp)
                                : item.Html.EditorFor(m => m.FemaleProp))
            </div>,
            @<col style="color: darkred" />,
            @<text>Male or female conditional editor</text>)
            
        // adding a table-level section
        .AddSection("Empty", @<text>Table is empty</text>)
        ;
}

// Continue reading! You still need to render the created template...
```

Next, there is a case use of `Table`, passing the previously created `TableTemplate`.
This is where the template is rendered into a table frame.
This could be defined in another file such as a custom helper (defined in the App_Code folder):

```csharp
@helper MyCustomTable(HtmlHelper html, ITableTemplate tableTemplate)
{
    var table = html.Table(tableTemplate);
    <table id="@table.Html.ViewData.TemplateInfo.GetFullHtmlFieldId("")">
        @{
            // getting all items... just testing (so to show that it is possible)
            // and rendering these models to JSON
            var allItems = table.Items.Select(item => item.Html.ViewData.Model).ToArray();
            <script type="text/javascript">
                Models = @(html.Raw(new JavaScriptSerializer().Serialize(allItems)));
            </script>

            var header = table.Header;
            <colgroup>
                @foreach (var headerColumn in header.Cells)
                {
                    // rendering the section "ColumnGroup",
                    // passing a default HTML to use when the section is not defined
                    @headerColumn.RenderSection("ColumnGroup", @<col />)

                    // rendering something using `IsSectionDefined`
                    if (headerColumn.IsSectionDefined("ColumnGroup"))
                    {
                    <!--Section "ColumnGroup" was defined for the previous column-->
                    }
                }
            </colgroup>
            <thead>
                <tr>
                    @foreach (var headerColumn in header.Cells)
                    {
                        // Rendering section "HeaderHint"... this section is required
                        // since the single parameter overload is used.
                        // Then we render the content of the header cell with `Render`.
                        <th title="@headerColumn.RenderSection("HeaderHint")">
                            @headerColumn.Render()
                        </th>
                    }
                </tr>
            </thead>
        }
        @if (table.Items.Any())
        {
            // rendering table items, if any
            <tbody>
                @foreach (var item in table.Items)
                {
                    <tr id="@item.Html.ViewData.TemplateInfo.GetFullHtmlFieldId("")">
                        @item.RenderIndexHiddenField()
                        @foreach (var itemColumn in item.Cells)
                        {
                            // rendering table data cells
                            <td id="@itemColumn.Html.ViewData.TemplateInfo.GetFullHtmlFieldId("Cell")">
                                @itemColumn.Render()
                            </td>
                        }
                    </tr>
                }
                @{
                // rendering a row that allows new items to be inserted
                var newItem = table.NewItem(int.MaxValue, null); 
                    <tr>
                        @newItem.RenderIndexHiddenField()
                        @foreach (var itemColumn in newItem.Cells)
                        {
                            <td>
                                @itemColumn.Render()
                            </td>
                        }
                    </tr>
                }
            </tbody>
        }
        else
        {
            // rendering the Empty table-level section
            <tbody>
                <tr>
                    <td colspan="@table.Header.Cells.Count()">@table.RenderSection("Empty")</td>
                </tr>
            </tbody>
        }
    </table>
}
```

Finally, we can use this helper to render a table in the page:

```csharp
@MyCustomTable(this.Html, tableTemplate)
```
