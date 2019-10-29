
![N|Solid](https://i.imgur.com/GePjAV6.png)

# Nimble Search for Sitecore

Nimble Search is a Sitecore open source module built on top of Sitecore content search API, and currently Supprt Sitecore implementation for Solr indexing, The module helps developers quickly build a search page with essential search UI elements such as results, facets, search bar with auto complete, paging, sorting and other out of the box, With minimal code needed. 

# Main Features
  - **Search filters and settings** are stored as sitecore items where you can select you sitecore index name, start locations, filtered templates additional field filters, Page views (Yet to be implemented), sort options and many others.
  - **Restful endpoint for nimble search** that return search results in json format
  - **Extensible implementation** by using Sitecore pipelines for building query, Applying facets, boosting and mapping results where you can add your processor for additional changes, though the out of the box processors are sufficient enough for advanced search operations
  - **Search UI markup** built using [handlebars.js], a popular templating engine that is powerful, simple to use and has a large community, where you can separate the generation of the html from the rest of your code.

# Search Filter and Settings
Nimble search let you set your search filters and settings directly in sitecore as items, We have a branch template that creates a set of initial search items that you need to start with, and then you can use the search tab item as a datasource for your nimble search template.
![N|Solid](https://i.imgur.com/uGWSRU9.png)
---------
----
 There  are different items need to be created to build your search filters and settings:
###   ( Tab Item )
First, you need to create your search tab item as a datasource item, which has the following fields
  - **Index Name**: Your solr index name inside your sitecore Solr Content Search configurations
  - **Apply Security** : To apply security restriction for logged in users
  - **Templates**: To only search items of the selected templates (Or inherit from it)
  - **Query Filter**: Sitecore rule that let you add additional filters on fields using Rule engine
  - **Keyword Fields**: Nimble search will search against `_content` field in Solr index, you can use this field to include additional computed fields to search against (Pipe delimited)
  - **Latest Version Only**: checkbox to search only for latest version
  - **Current Language only**: checkbox to search against on context language
  - **Autocomplete Field Name**: This is a solr field name for which you want your autocomplete to search against
  - **Page Views**: To select between list or tile view for your search results, This points to predefined list of page views stored in `/sitecore/system/Settings/Foundation/NimbleSearch/View Options`, Each view have additional settings such as page sizing options and css class name to be used in the rendered search results
  - **Default Page View**: The default page view to show on initial page load.
  - **Sorting Options** : Sort options that user can select to sort the search result, this points to predefined list of sort options stored in `/sitecore/system/Settings/Foundation/NimbleSearch/Sort Options/Common`, Each sort option have the field name in Solr to sort against and checkbox for Descending sorting.
  - **Default Sorting Option**: The default sort option to use at page initial load.
  - **Result Field**: Nimble search endpoint returns basic fields for each search result such as name, url, template, image etc, but here you can define a list of pipe separated field names that you want to return from solr.
  - **Register Search Term** : checkbox to register search keyword into Sitecore xdb database.
  - **Empty Results Message**: a rich text field to show a message for users when no results are found.

### ( Facet Item )
If you want to include facets for your search, you need to add a facet definition item, your sorl index field need to be already included in your search configurations, This item need to be created under your `tab item/Facets` folder, facets will appear on search page in the same order they are created in sitecore, You need to fill the following fields for each facet item:
  - **Field Name**: field name in your index configuration
  - **Sort Facet Values**: Drop down to select the sorting of the facet values, options available are Occurrences, Alphabetical Ascending and Alphabetical Descending.

### ( Boost Item )
If you want to boost your serahc results based on specific fields, you can create boost items directly under `tab item/ Boosting` folder, boost item will have the following fields:
  - **Boost Field**: field name in your index configuration
  - **Boost Amount**: numberic value between -100 to 100 to select for boosting

# Restful endpoint for nimble search
Nimble search process the filters defined in Sitecore items along with user search input (keyword, selected facets, sort, paging) and return the results in json format, to view search result format for your defined search tab item, open this url in your browser `/api/nimble/search/{Your tab item guid}`.
For example, this is the json returned for each search result item:

```json
{
  ItemID: "f30c4470-07d4-4652-9cb6-bf8ac43d1694",
  Title: "Secure Page",
  Summary: "This is a secure page. You have to be logged in to see this page.",
  Url: "http://nimble.local.test/Secure Page",
  Type: "Article",
  ImageUrl: "/-/media/Habitat/Images/Square/Default.jpg?as=1&mh=200&mw=200&hash=810BD5C618B45BBDBF1C5DFFA27C19F1",
  Custom: {
    title: "Secure Page",
    summary: "This is a secure page. You have to be logged in to see this page."
    }
}
```

# Extensible implementation
Nimble Search implementation can be extended and modified to deliver a more custom search implementations, configurations and settings are located inside `NimbleSearch.Foundation.Core.Settings.config` file where you can change the default Name, Date, Image or Summary fields returned from the restful api.

# Search UI markup
Nimble Search UI markup uses handlebars.js for html templating, its very easy to use and change, See the following html used for the search results (Inside `Views\NimbleSearch\SearchResults.cshtml`):

```html
<script id="nimble-data-template" type="text/x-handlebars-template">
    <ul class="list-unstyled list-divided">
        {{#each Items as |item|}}
        {{#with item.Custom as |property|}}
        <li>
            <div>
                <span class="label label-info">{{item.Type}}</span>
            </div>
            <h2>
                <a href="{{item.Url}}">{{property.title}}</a>
            </h2>
            {{property.summary}}
        </li>
        {{/with}}
        {{/each}}
    </ul>
</script>
```

With the json object returned for each result from Nimble Search endpoint, you can build your html markup with handlebars.js the way you wanted, even writing if statements or handlebar functions for advanced templating


### Installation

Currently Nimble Search is supporting Sitecore 9+ with Solr indexing enabled, Future commits will add support for Azure search.

To Install Nimble Search, 
  - download the package from releases page and install it in sitecore
  - Create your search tab item and other setting items, You can use branch template `/sitecore/templates/Branches/Foundation/NimbleSearch/Search Configurations` to create predefined structure, you can create this anywhere in content tree, but usually you could add it under your search page as **local** datasource item.
  - Fill in the fields in tab item and create your facets and boosting items decribed in previous section.
  - Go to your search page, and open presentation layer and add rendering `/sitecore/layout/Renderings/Foundation/NimbleSearch UI/Search Template` to your main placeholder, Select your search tab as datasource (Tab item is of template `/sitecore/templates/Foundation/NimbleSearch/Datasource/Tab`)
  - Publish your site and open your search page.
  - To modify any html markup, You can go to any of the following razor views and update the html :
    - **\Views\NimbleSearch\SearchTemplate.cshtml**  : This is the main container for all the search UI elements (as partial views), you can move each element to different place, Removing any of the below elements does not affect the search functionality, eg removing facets
    - **\Views\NimbleSearch\SearchResults.cshtml**  : This is the template for search results
    - **\Views\NimbleSearch\SearchFacets.cshtml**  : This is the template for search facets
    - **\Views\NimbleSearch\SearchBox.cshtml**  : This is the template for search box
    - **\Views\NimbleSearch\NumberedPagination.cshtml**  : This is the template for search pagination
    - **\Views\NimbleSearch\DropDownSort.cshtml**  : This is the template for search sorting

### Development

Want to contribute? Great, we need you!

Let us know if you want to be part of nimble search team, the more help we get the better we deliver a more optimized Sitecore Search UI to the community.


### Todos

 - Improve UI elements styling, currently Nimble search does not include css styling for desktop or mobile
 - Fix js issues
 - Improve JS code to include events triggered at certain events, such as onQueryBuild, OnRenderComplete etc
 - Implement Azure Search
 - Implement different paging options with different handlebar templates for each view
 - Add support for mutiple search tabs

License
----

MIT

   [handlebars.js]: <https://handlebarsjs.com/>
   
