## Design and architecture

### Domain model

Provide an illustration of your domain model.
Make sure that it is correct and complete.
In case you are using ASP.NET Identity, make sure to illustrate that accordingly.

### Architecture — In the small

Below a diagram can be seen, showing the onion architecture of the Chirp program. Were the outer circles depend on the inner circles:

<figure style="text-align: center;">
    <img src="images/Architecture%20—%20In%20the%20small.drawio.png" style="width: 50vw;"/>
    <figcaption>Figure 1. Onion architecture of Chirp program</figcaption>
</figure>

In the center of the onion one finds Chirp Core, this is were the most primitive code lies, like objects and interfaces.

One step out of Chirp Core, one finds Chirp Infrastructure. This is were the handling of the database is done, this includes retrieving, deleting and updating data. Defining the database and giving it some initial data is also done in Chirp Infrastructure.

In the third layer of the onion, Chirp Web lies. This is were the webpage HTML is found, along with all the styling. The API which the web pages communicate with lies here as well.

Lastly there is the outer layer, naturally here the test lay. The test suit includes Unit-, integration- and end2end test. The end2end test are done using Playwright.

### Architecture of deployed application

Illustrate the architecture of your deployed application.
Remember, you developed a client-server application.
Illustrate the server component and to where it is deployed, illustrate a client component, and show how these communicate with each other.


### User activities

The goal of this chapter is to show some core interactions from both **unauthenticated user** and **authenticated user**. We make use of UML activity diagrams, these will visualize the states triggered by a users actions.

First off we want to show what a unauthenticated user can do, and how the journey is for such users to get to register.
<figure style="text-align: center;">
    <img src="images/chirpUserActRegister.png" style="width: 50vw;"/>
    <figcaption>Figure 2. unauthenticated user journey and register</figcaption>
</figure>

This diagram show that a user can authenticate with both Email, and GitHub. Also if you like a cheep from a user on the public timeline. It will simply not like it, but instead put you on the register page. Registering this way will give the same result as just navigating to the register page using the navigation bar.

When you are authenticated / logged in, we have 4 primary action a user can do, repectively: Cheep, Like, Follow and Delete the account from the Chirp service.

The process of cheeping is show in this diagram:

<figure style="text-align: center;">
    <img src="images/chirpUserActCheep.png" style="width: 50vw;"/>
    <figcaption>Figure 2. Cheep journey and validation of cheep</figcaption>
</figure>

A cheep is valid if its length, as show in the diagram, is between 0 and up to and including 160 characters. If you were to click the Share button, with and empty text field, a warning will pop up. A warning pop up wont explicitly be shown to the user for cheeps longer that 160 characters, we simply show the length counter on screen, and dont allow for more characters, in both front- and backend.

The users also need to like cheeps, for that action we have this diagram:

<figure style="text-align: center;">
    <img src="images/chirpUserActLike.png" style="width: 50vw;"/>
    <figcaption>Figure 2. Cheep journey and validation of cheep</figcaption>
</figure>






### Sequence of functionality/calls trough _Chirp!_

With a UML sequence diagram, illustrate the flow of messages and data through your _Chirp!_ application.
Start with an HTTP request that is send by an unauthorized user to the root endpoint of your application and end with the completely rendered web-page that is returned to the user.

Make sure that your illustration is complete.
That is, likely for many of you there will be different kinds of "calls" and responses.
Some HTTP calls and responses, some calls and responses in C# and likely some more.
(Note the previous sentence is vague on purpose. I want that you create a complete illustration.)

## Process

### Build, test, release, and deployment

Illustrate with a UML activity diagram how your _Chirp!_ applications are build, tested, released, and deployed.
That is, illustrate the flow of activities in your respective GitHub Actions workflows.

Describe the illustration briefly, i.e., how your application is built, tested, released, and deployed.

### Team work

Show a screenshot of your project board right before hand-in.
Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.

Briefly describe and illustrate the flow of activities that happen from the new creation of an issue (task description), over development, etc. until a feature is finally merged into the `main` branch of your repository.

### How to make _Chirp!_ work locally

There has to be some documentation on how to come from cloning your project to a running system.
That is, Adrian or Helge have to know precisely what to do in which order.
Likely, it is best to describe how we clone your project, which commands we have to execute, and what we are supposed to see then.

### How to run test suite locally

List all necessary steps that Adrian or Helge have to perform to execute your test suites.
Here, you can assume that we already cloned your repository in the step above.

Briefly describe what kinds of tests you have in your test suites and what they are testing.

## Ethics

### License

State which software license you chose for your application.

### LLMs, ChatGPT, CoPilot, and others

State which LLM(s) were used during development of your project.
In case you were not using any, just state so.
In case you were using an LLM to support your development, briefly describe when and how it was applied.
Reflect in writing to which degree the responses of the LLM were helpful.
Discuss briefly if application of LLMs sped up your development or if the contrary was the case.
