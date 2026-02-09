# QuickBase-project

I have created a new web project called WorldPopulationMVC, because I wanted be able to visualise 
the information on a web page. To start it just set WorldPopulationMVC as a startup project and run it
the browser should open automatically on this url https://localhost:7030/ and it should display all the 
aggregated data. 

A few things to consider. The data set which is hardcoded in ConcreteStatService, I consider records like
"U.S. Virgin Islands (US)" as data about a particular city and not as data for the entire country and also 
records like "United States of America" as data for the entire country. So the data in "United States of America" 
will overwrite the data in the "U.S. Virgin Islands (US)". I have also added more tables to the database to be able 
to save different alternative names for the countries as in your example they might not be exactly the same in all 
data sources. 

The data sources can be ranked, the solution is configured to take priority of the data in the db and put the data from 
the hardcoded data set as second. The priority can be configured from the appsettings.

