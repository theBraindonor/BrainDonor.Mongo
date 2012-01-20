BrainDonor Mongo Utilities
==========================

The BrainDonor Mongo Utilities allow for the quick creation of data-driven ASP.NET MVC3 applications running on top of MongoDB:
+ A simple repository model to take out a lot of the book-keeping behind the mongo collections and get you coding faster.
+ Membership and Role providers that implment the ASP.NET Application Services model.

MongoDB Repository
------------------

The goal of the repository objects, BrainDonor.Mongo.Data, are to provide a quick abstraction to create objects that are mapped to MongoDB collections.  The MVC Binders necessary to utilize the BSON ObjectId data type in route data has also been included.

MongoDB Membership and Role Providers
-------------------------------------

These membership and role providers utilize the mongo reposity model to allow for the easy creation of User and Role collections in MongoDB that can be wired up the standard ASP.NET Forms Authentication Providers.  These providers can be used in both WebForms and MVC projects.
