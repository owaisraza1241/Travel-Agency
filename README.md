FlightSearchApp FlightSearchApp is a .NET Core 8 based application designed to search for flights and hotels using the Amadeus API. The application consists of multiple projects including a TravelAgency for user interaction, an infrastructure project for API communication, domain models for handling flight and hotel data.

Table of Contents Prerequisites Setup Configuration Usage Testing Logging Project Structure Contributing License Prerequisites .NET 8 SDK or later Visual Studio 2022 or later / Visual Studio Code api.amadeus.com credentials Setup Clone the repository:

First you need to create the account on https://test.api.amadeus.com

And then copy the client id and client secrect and paste it in the appsetting.json file in the TravelAgency project

