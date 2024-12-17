---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2024 Group 4
author:
- Allan Petersen <allp@itu.dk>
- Bergur Davidsen <berd@itu.dk>
- Lucas Venø Hansen <lucasveha@gmail.com>
- Mikkel Clausen <mikcl@itu.dk>
- Victor Sforzini <visf@itu.dk>
numbersections: true
---


## Design and architecture

### Domain model

### Architecture — In the small

Below a diagram can be seen, showing the onion architecture of the Chirp program. Were the outer circles depend on the inner circles:

![Onion architecture of Chirp program](images/Architecture — In the small.drawio.png){ width=50% }

In the center of the onion one finds Chirp Core, this is were the most primitive code lies, like objects and interfaces.

One step out of Chirp Core, one finds Chirp Infrastructure. This is were the handling of the database is done, this includes retrieving, deleting and updating data. Defining the database and giving it some initial data is also done in Chirp Infrastructure.

In the third layer of the onion, Chirp Web lies. This is were the webpage HTML is found, along with all the styling. The API which the web pages communicate with lies here as well.

Lastly there is the outer layer, naturally here the test lay. The test suit includes Unit-, integration- and end2end test. The end2end test are done using Playwright.

### Architecture of deployed application

![Architecture of deployed application](images/deployment_diagram.drawio.png)

The deployed application follows the client-server architecture. The client communicates with the server through HTTP requests. The server is hosted on the Azure App service and the database is sqlite. Communication between the server and the database is done through Entity Framework Core.

### User activities

The goal of this chapter is to show some core interactions from both an **unauthenticated user** and **authenticated user**. We make use of UML activity diagrams, these will visualize the states triggered by a user's actions.

First off we want to show what an unauthenticated user can do, and how the journey is for such a user to register.

![Unauthenticated user journey and register](images/chirpUserActRegister.png){ width=50% }

This diagram show that a user can authenticate with both Email, and GitHub. Also, if you like a cheep from a user on the public timeline. It will simply not like it, but instead put you on the register page. Registering this way will give the same result as just navigating to the register page using the navigation bar.

When you are authenticated / logged in, we have 4 primary actions a user can do, respectively: Cheep, Like, Follow and Delete the account from the Chirp service.

The process of cheeping is shown in this diagram:

![Cheeping journey and validation of cheep](images/chirpUserActCheep.png){ width=50% }

A cheep is valid if its length, as show in the diagram, is between 0 and up to and including 160 characters. If you were to click the share button, with and empty text field, a warning will pop up. A warning pop up won't explicitly be shown to the user for cheeps longer than 160 characters, we simply show the length counter on screen, and don't allow for more characters, in both front- and backend.

The users also need to like cheeps, for that action we have this diagram:

![CLiking cheeps](images/chirpUserActLike.png){ width=50% }

The 'heart' button we have besides each cheep is essentially a toggle for likes on the given cheep. And as showed in the diagram, each user can only like any given cheep once. It is important to note, as of now the liking of a cheep will result in the page redirecting you to the root page (page 1), even though you might be on for instance page 6. There is an obvious room for improvement, and the task is currently a task in the project board.

Next up we want to show the journey of a user following another user.

![Following users](images/chirpUserActFollow.png){ width=50% }

The journey of following a user, is close to the same as liking cheeps, as both are 'toggles'. The only difference is that we decided to show the newly followed users profile after the follow action. Which eliminates the issue we are having with liking cheeps far down on the public timeline, and wanting to scroll beyond that point afterwards. This does then create the issue with wanting to continue scrolling after following.
But this navigation to the private timeline of the newly followed user, is a conscious decision.

Lastly it is important for us to show how the user can delete, and see the data we have gathered.

![Deleting the user and download data](images/chirpUserActDelete.png){ width=50% }

The linear diagram is pretty much self-explanatory, but we feel it is important to show either way, since this is last key feature for a user to experience.

### Sequence of functionality/calls trough _Chirp!_

## Process

### Build, test, release, and deployment

All of the build, test, release, and deployment is done using GitHub Actions.

![Build and test flow](images/build.drawio.png){ height=70% }

The build and test flow is one out of two flows that run when a pull request is made to the main branch. This flow will build and run the test suit, and if the test suit passes, the flow will be marked as successful. If the test suit fails, the flow will be marked as failed.

![Playwright test flow](images/Playwright.drawio.png){ height=70% }

The Playwright test flow is the second flow that run when a pull request is made to the main branch. This flow will build and run the UI tests and end2end test with Playwright, and if the test suit passes, the flow will be marked as successful. If the test suit fails, the flow will be marked as failed.

![Deployment flow](images/deploy_action%20_flow.drawio.png){ height=70% }

The deployment flow is the flow that runs when a pull request is merged into the main branch. This flow will build the project, run the ```dotnet publish``` command, and deploy the project to the Azure App Service.

![Release flow](images/release_flow.drawio.png){ height=70% }

The release flow is the flow that runs when a version is tagged in the repository. This flow will build the project, run the ```dotnet publish``` command, and then create a zip folder with the _Chirp.Web.dll_ file. This zip folder is then uploaded to the GitHub release page under the tag that was created.

### Team work

Show a screenshot of your project board right before hand-in.
Briefly describe which tasks are still unresolved, i.e., which features are missing from your applications or which functionality is incomplete.

Briefly describe and illustrate the flow of activities that happen from the new creation of an issue (task description), over development, etc. until a feature is finally merged into the `main` branch of your repository.

![Flow of activites, issue to merge](images/ProjectboardFlow.png){ width=75% }

### How to make _Chirp!_ work locally

### How to run test suite locally

## Ethics

### License

We decided to go ahead with and use the **MIT License**

### LLMs, ChatGPT, CoPilot, and others

In the development of our project we used ChatGPT, and when we did so, we made sure to add ChatGPT as a co-author in our git commit message like so:

`ChatGPT <>`

ChatGPT was very helpful when used to create simple code parts and debug some.
On the other hand the ChatGPT was not helpful with complex code questions. Therefore, we ended up finding it mostly useful for us to understand parts of the code and guide us on where to start on complex tasks.

However, we also experienced some negatives when using ChatGPT. It could sometimes go in a spiral, in cases like this we would look at the slides and on the web for help.
We also gave Gemini some use sometimes when ChatGPT was not helpful, we did however never use any of the provided code, so it never got to be a co-author.

For the most part the use of LLMs sped up our development, however sometimes they were sent into a spiral and hallucinated, which could confuse us more.
So we experienced the limitations of LLMs and got to learnt how to use them more efficiently.
