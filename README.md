BrainDonor MongoDB Utilities
==========================

The BrainDonor MongoDB Utilities allow for the quick creation of data-driven ASP.NET MVC3 applications running on top of MongoDB:

* A simple repository model to take out a lot of the book-keeping behind the mongo collections and get you coding faster.
* Membership and Role providers that implment the ASP.NET Application Services model.


MongoDB Repository
------------------

The goal of the repository objects, BrainDonor.Mongo.Data, are to provide a quick abstraction to create objects that are mapped to MongoDB collections.  The MVC Binders necessary to utilize the BSON ObjectId data type in route data has also been included.

Implementing an object that will be stored in MongoDB is as follows:

        [Serializable]
        public class Customer : Item<Customer>
        {
            [BsonId]
            public ObjectId CustomerId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Title { get; set; }
            public Address Address { get; set; }
            public string PhoneNumber { get; set; }
            public string EmailAddress { get; set; }
        }

        [Serializable]
        public class Address
        {
            public string StreetAddress1 { get; set; }
            public string StreetAddress2 { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string ZipCode { get; set; }
        }

Each Customer object will be stored in the Customer collection in MongoDB.  Each Customer object will have a primary key of CustomerId--but this primary will key will be stored in the _id field inside MongoDB.

By default, the model assumes that the DefaultMongoDB connection string exists in the Web.config or App.config:

		<connectionStrings>
			<add name="DefaultMongoDB" connectionString="mongodb://localhost/BraindonorMongoExample" />
		</connectionStrings>

MongoDB Membership and Role Providers
-------------------------------------

These membership and role providers utilize the mongo repository model to allow for the easy creation of User and Role collections in MongoDB that can be wired up the standard ASP.NET Forms Authentication Providers.  These providers can be used in both WebForms and MVC projects.

Implementing the Membership and Role providers in your application is as easy as:

		public class UserProvider : MongoMembershipProvider<User, Role> { }
		public class User : MongoUser<User> { }
		public class RoleProviders : MongoRoleProvider<User, Role> { }
		public class Role : MongoRole<Role> { }

The MongoUser and MongoRole classes are built on top of the repository model.  Any properties that you add to your implementation class will appear in the resulting Bson document representing the class in MongoDB.

Example Website
--------------

An example website is included to demonstrate the utilities in action.  This example assumes that MongoDB is up and running on localhost.  The example utilizes the .NET MVC3 Internet Application skeleton.  I have added the following to the project:

* Dependancies necessary to reference the MongoDB utilities.
* Implemented the providers in the Example.Web.Data namespace.
* Added the MongoDB connection string and providers to Web.config.
* Added a default credentials and role assignment so that you can log in with an empty mongo database.
* Added the Bson ObjectId binder to Global.asax.
* Added the Admin area for user and role administration which is built using Bootstrap from Twitter.
