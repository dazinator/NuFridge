**NuFridge is a web application for managing multiple NuGet feeds.**

NuFridge is useful for anyone that wants to easily be able to get started with creating and managing NuGet feeds.

The NuGet feeds that are created are Klondike based feeds: https://github.com/themotleyfool/Klondike which means they use Lucene indexing for lightning fast performance.

## Features ##

 - Create multiple NuGet feeds
 - Retention policies for packages

## Requirements ##
 - IIS (Internet Information Services)
 - .NET Framework 4.5.1

## Installation ##

 1. Create a website in IIS to host NuFridge
 2. Create a website in IIS to host the NuGet Feeds (optional - it will be created automatically)
 3. Edit the web.config app settings to have the correct MongoDB connection string and database name
 4. Edit the web.config app settings to have the correct NuGet feed website information from step 2
 5. Browse to the NuFridge website