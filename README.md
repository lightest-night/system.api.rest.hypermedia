# Lightest Night
## API > REST > Hypermedia

Functions, Utilities and Helpers that enable the use of Hypermedia as the Engine of Application State (HATEOAS).

### Build Status
![](https://github.com/lightest-night/system.api.rest.hypermedia/workflows/CI/badge.svg)
![](https://github.com/lightest-night/system.api.rest.hypermedia/workflows/Release/badge.svg)

#### How To Use
##### Registration
* Asp.Net Standard/Core Dependency Injection
  * Use the provided `services.AddHypermedia(string vendorName) method`
    * The vendorName string is used to build your HATEOAS accepted media type values (e.g. application/vnd.[VENDORNAME].hateoas+json)

* Other containers
  * Register your supported media types into the relevant output formatters
  * Register the Web Api controllers, adding the `ValidateMediaTypeAttribute` as a filter; alternatively add [ServiceFilter(typeof(ValidateMediaTypeAttribute))] to your controllers.
  * Register your output formatters into the container
  
##### Usage
* Create a mapping class inheriting from `ControllerMap<TController, TReadModel>` and in the controller call `CreateLinkDefinition` for all the different HATEOAS links your `TReadModel` will need
* Make your `TController` inherit from `HypermediaControllerBase` and return as normal. `Ok` and `Created` IActionResults will automatically detect the defined links and add them to your response.
* NB: If the accept header does not include a valid HATEOAS media type, then the response will not be amended to include the links