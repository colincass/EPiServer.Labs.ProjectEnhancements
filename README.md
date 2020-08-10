# Episerver Labs - Project Enhancements

The project contains a few features that, in general, make the life of Episerver editors easier.

The list of current features is as following:
* [Page tree indicator](#page-tree-indicator)
* [Selected project tooltip](#selected-project-tooltip)
* [Project description](#project-description)
* [Project categories](#project-categories)
* [Notification tooltip](#notification-tooltip)

## Install

Episerver Labs Project Enhancements is available as a [Nuget](https://nuget.episerver.com/package/?id=EPiServer.Labs.ProjectEnhancements) package for CMS add-ons.
To use it in a project, install the `EPiServer.Labs.ProjectEnhancements` package:

```console
Install-Package EPiServer.Labs.ProjectEnhancements
```

## Development in this repository

```console
>> build\tools\nuget.exe restore
>> setup.cmd
>> build.cmd
>> test.cmd
>> site.cmd
```

## Page tree indicator

![Page tree indicator](assets/docsimages/page_tree_indicator.png)

## Selected project tooltip

![Selected project tooltip](assets/docsimages/selected_project_tooltip.png)

## Project description

![Project dialog](assets/docsimages/project_description_dialog.png)

![Project dialog overview](assets/docsimages/project_description_overview.png)

![Project dialog list](assets/docsimages/project_description_list.png)

## Project categories

![Project categories](assets/docsimages/project_categories.png)

![Project categories list](assets/docsimages/project_categories_list.png)

## Notification tooltip

![Notification tooltip](assets/docsimages/project_notification_tooltip.png)

## Options

All features can be turn on and off using ProjectOptions class:

````
[InitializableModule]
public class ProjectOptionsInitialization : IConfigurableModule
{
    public void ConfigureContainer(ServiceConfigurationContext context)
    {
        context.ConfigurationComplete += (o, e) =>
        {
            context.Services.Configure<ProjectOptions>(options =>
            {
                options.ShowPageTreeIndicator = false;
                options.ShowDescription = false;
                options.ShowCategories = false;
                options.ShowNotificationTooltip = false;
                options.ShowSelectedProjectTooltip = false;
            });
        };
    }

    public void Initialize(InitializationEngine context)
    {
    }

    public void Uninitialize(InitializationEngine context)
    {
    }

    public void Preload(string[] parameters)
    {
    }
}
````

By default all options are enabled.

## Telemetry information

In a quest to understand our users more and effectivize our resources so that we can deliver the best user experience possible, we've decided to gather some useful telemetry so that we can make more informed decisions on what we should improve, support
and maybe not pursue when developing new features for CMS UI. We assure that the data we collect is completely anonymized and will only be used internally for making decisions on improving the user experience.

If you allow us to learn more about what we should be building, please make sure these two URL's are not blocked:

* Live Configuration: `https://cmsui.episerver.net/api/telemetryconfig`
* Tracking: `https://dc.services.visualstudio.com/v2/track` (this can change on Microsoft's discretion)

### Taxonomy of custom events

#### Always included

Every tracking event includes [standard Application Insights dimensions](https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#trackevent). The [authenticated user and client ID](https://docs.microsoft.com/en-us/azure/azure-monitor/app/api-custom-events-metrics#authenticated-users) are set as:

* `ai.user.authUserId`: String, a SHA512 hash without salt, using user email if available and username otherwise. To anonymize user but allow tracking between products.
* `ai.user.accountId`: String, a SHA512 hash without salt, using the License key. To allow for grouping of users.

Additionally we are tracking:

[TODO: ]

### Please note
> Episerver Labs projects are meant to provide the developer community with pre-release features with the purpose of showcasing ongoing work and getting feedback in early stages of development.
>
> You should be aware that Labs are trials and not supported Episerver releases. While we hope you use them, there are a few things you should expect from each release:
> - Functionality may be added, removed, or changed.
> - Labs projects have not been through the same quality assurance process as the versioned products and, in some cases, may show unexpected behavior.
>   - The Episerver CMS UI team notes that:
>     - the scenarios in the Readme of each CMS Lab's repo will be verified and supported us
>     - the Labs add-on may or may not work with other add-ons, we are not testing them
>     - any such issues found, such as scenarios outside of the the Readme, can be fixed by the community by submitting a Pull Request on the Github repo
> - The software may not work on all environments.
>   - The Episerver CMS UI team notes that:
>     - Although it should work on base installations of CMS UI in Chrome and Firefox
> - There is no Episerver support; however, we will gratefully receive all feedback
>   - The Episerver CMS UI team notes that:
>     - Issues created on GitHub will be triaged, and if accepted, fixed by us
>
> The software is provided â€œAs isâ€ without warranty of any kind or promise of support. In no event shall Episerver be liable for any claim, damages or liability in relation to the software. By using this software you are also agreeing to our developer program terms [https://www.episerver.com/legal/program-terms/](https://www.episerver.com/legal/program-terms/)