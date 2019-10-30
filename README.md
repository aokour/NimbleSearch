<p align="center">
  <img  src="https://i.imgur.com/GePjAV6.png">
</p>


# Nimble Search for Sitecore

Nimble Search is a Sitecore open source module built on top of Sitecore content search API, and currently supports Sitecore's implementation for Solr indexing. The module helps developers quickly build a search page with essential search UI elements such as results, facets, search bar with auto complete, paging, sorting out of the box, only needing CSS styling. It offeres a framework with simplicity and extensibility in mind to support even custom needs with minimal effort. 

# Main Features
  - **Search filters and settings** are stored as sitecore items where you can select you sitecore index name, start locations, filtered templates additional field filters, Page views (Yet to be implemented), sort options and many others.
  - **Restful endpoint for nimble search** that returns search results in json format
  - **Extensible implementation** by using Sitecore pipelines for building query, Applying facets, boosting and mapping results where you can add your processor for additional changes, though the out of the box processors are sufficient enough for advanced search operations
  - **Search UI markup** built using [handlebars.js], a popular templating engine that is powerful, simple to use and has a large community, where you can separate the generation of the html from the rest of your code.

# Search Filter and Settings

 Nimble Search lets you set your search filters and settings directly in sitecore as items. We have a branch template that creates the Sitecore datasource and initial set of setting subitems that you need to start with, and then you can tailor as desired to manage facets, sorting, boosting, custom result fields, etc. Use the search tab item as the datasource for your nimble search template rendering.

![N|Solid](https://i.imgur.com/uGWSRU9.png)
---------
----
 There  are different items that need to be created to build your search filters and settings:
###   ( Tab Item )
First, you need to create your search tab item as a datasource item, which has the following fields
  - **Index Name**: Your solr index name inside your sitecore Solr Content Search configurations
  - **Apply Security** : To apply security restriction for logged in users
  - **Templates**: To only search items of the selected templates (Or inherit from it)
  - **Query Filter**: Sitecore rule that lets you add additional filters on fields using Rule engine
  - **Keyword Fields**: Nimble search will search against `_content` field in Solr index, you can use this field to include additional computed fields to search against (Pipe delimited)
  - **Latest Version Only**: checkbox to search only for latest version
  - **Current Language only**: checkbox to search against on context language
  - **Autocomplete Field Name**: This is a solr field name for which you want your autocomplete to search against
  - **Page Views**: To select between list or tile view for your search results, This points to predefined list of page views stored in `/sitecore/system/Settings/Foundation/NimbleSearch/View Options`, Each view have additional settings such as page sizing options and css class name to be used in the rendered search results
  - **Default Page View**: The default page view to show on initial page load.
  - **Sorting Options** : Sort options which user can select to sort the search results, this points to predefined list of sort options stored in `/sitecore/system/Settings/Foundation/NimbleSearch/Sort Options/Common`. Each sort option has the index field name in Solr to sort against and option for Descending order. Add more as desired.
  - **Default Sorting Option**: The default sort option to use at page initial load.
  - **Result Field**: Nimble search endpoint returns basic fields for each search result such as name, url, template, image etc, but here you can define a list of pipe separated field names that you want to return from solr.
  - **Register Search Term** : checkbox to register search keyword into Sitecore xdb database.
  - **Empty Results Message**: a rich text field to show a message for users when no results are found.

### ( Facet Item )
If you want to include facets for your search, you need to add a facet definition item. This index field must already live in your solr index. An item needs to be created under your `tab item/Facets` folder. Facets will appear on the search page in the same order they are created in sitecore. You need to fill the following fields for each facet item:
  - **Field Name**: field name in your index configuration
  - **Sort Facet Values**: Drop down to select the sorting of the facet values, options available are Occurrences, Alphabetical Ascending and Alphabetical Descending.
#### Extending Facets
If new facet widget is desired for new UI requirements, a new facet definition can be added.
 - Added new Facet Type template
 - Implement a **FacetOn** pipeline processor for to call args.Query.FacetOn(...)
 - Implement a **SelectedFacet** pipeline processor to call args.Query.Where(...)
 -- Extend the following for both: `NimbleSearch.Foundation.Abstractions.Pipelines.ApplyFacet.ApplyFacetProcessor, NimbleSearch.Foundation.Abstractions`
 - Facet model has a "Custom" collection named you can leverage to pass key values pairs around.

### ( Boost Item )
If you want to boost your search results based on specific fields, you can create boost items directly under `tab item/ Boosting` folder. Boost items will have the following fields:
  - **Boost Field**: field name in your index configuration
  - **Boost Amount**: numberic value between -100 to 100 to select for boosting

# Restful endpoint for nimble search
Nimble search processes the filters defined on Sitecore items along with user input (keyword, selected facets, sort, paging) and returns the results in json format. To sample json search results, open this url in your browser `/api/nimble/search/{Your tab item guid}`.
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


# Best Data
Common data for search results include
- Title
- Summary
- Image
- Date

It is not uncommon that these desired values are stored in different fields for different sitecore templates.
To simplify gathering this data for search results, there is a config to map Sitecore fields to these data points.
For example, to add fields "Summary", "Description", and "Text" for BestSummary:
```xml
<sitecore>
        <nimble>
            <settings type="NimbleSearch.Foundation.Core.Configuration.Settings, NimbleSearch.Foundation.Core">
                <bestSummaryFields hint="list">
                    <fieldName hint="summary">Summary</fieldName>
                    <fieldName hint="description">Description</fieldName>
                    <fieldName hint="default">Text</fieldName>
                </bestSummaryFields>
            </settings>
    </nimble>
</sitecore>
```
Use sitecore config patches to add your custom template field names to the lists. Order does matter. As the nimble computed field processes the sitecore item it will return the first field with a value. BestName and BestDate are also used for default Name and Date sort options. See the settings config for more info: `NimbleSearch.Foundation.Core.Settings.config`

# Extensible implementation
Nimble Search implementation can be extended and modified to deliver a more custom search implementations. The Nimble framework has several pipelines for easy extension points.

URL: **/api/nimble/search**\
Pipelines:
- **nimble.initQuery**: Sets the index to query
- **nimble.buildQuery**: given Tab Item settings and user input append clauses to the Sitecore Content Search Api Queryable
- **nimble.applyBoost**: given a boost item, apply boost to queryable
- **nimble.applyFacetOn**: given a facet item, apply the FacetOn call for each facet type
- **nimble.applySelectedFacet**: given a facet item, apply the query predicates for each facet type
- **nimble.mapResult**: given search results, map to JSON model

URL: **/api/nimble/analytics**\
Pipelines:
- **nimble.searchAnalytics**: Tab Item settings, user input for search, and search results, process for analytics
See the pipelines config for more info: `NimbleSearch.Foundation.Core.Pipelines.config`
These pipelines are called from the NimbleService class which can also be replaced via IoC dependency injection, though not expected to be necessary.

# Search UI markup
Nimble Search UI markup uses handlebars.js for html templating, its very easy to use and change. See the following html as an example for the search results (Inside `Views\NimbleSearch\SearchResults.cshtml`):

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

With the json object returned for each result from Nimble Search endpoint, you can build your html markup with handlebars.js the way you wanted, even writing if statements or handlebar functions for advanced templating. The markup layer is meant to be tailored, even duplicated for search page variations. Can use existing Sitecore nimble renderings or create your own. HTML can be changed as desired. The main thing is to keep the Nimble template name on the script tag, and the Nimble HTML placeholder element ID so the JS layer can execute the template for each area it finds.


### Installation

Currently Nimble Search is supporting Sitecore 9+ (tested on 9.2.0) with Solr indexing enabled, Future commits will add support for Azure search.

To Install Nimble Search, 
  - download the package from releases page and install it in sitecore
  - Create your search tab item and other setting items, You can use datasource branch template `/sitecore/templates/Branches/Foundation/NimbleSearch/Search Configurations` to create predefined structure, you can create this anywhere in content tree, but usually you could add it under your search page as **local** datasource item.
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
   
