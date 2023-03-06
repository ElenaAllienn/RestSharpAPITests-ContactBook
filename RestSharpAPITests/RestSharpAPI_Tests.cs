using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RestSharp;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://contactbook.elenaallienn.repl.co/api";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }

        [Test]
        public void Test_Contact_FirstAndLastName()
        {
            // Arrange
            var expectedFirstName = "Steve";
            var expectedLastName = "Jobs";
            var request = new RestRequest("contacts", Method.Get);

            // Act
            var response = this.client.Execute(request);

            var contacts = JsonSerializer.Deserialize<List<Contact>>(response.Content);
            var firstContact = contacts.FirstOrDefault();
            
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsNotNull(firstContact);
            Assert.That(firstContact.firstName, Is.EqualTo(expectedFirstName));
            Assert.That(firstContact.lastName, Is.EqualTo(expectedLastName));


            //Assert.That(contacts[0].firstName, Is.EqualTo("Steve"));
            //Assert.That(contacts[0].lastName, Is.EqualTo("Jobs"));

        }
        [Test]
        public void Test_SearchByKeyword_ValidResults()
        {
            // Arrange
            var expectedFirstName = "Albert";
            var expectedLastName = "Einstein";
            var request = new RestRequest("contacts/search/3", Method.Get);

            // Act
            var response = this.client.Execute(request);
            var contacts = JsonSerializer.Deserialize<List<Contact>>(response.Content);

            //softUni
            //var contact = JsonSerializer.Deserialize<List<ContactDTO>>(response.Content).FirstOrDefault(x => x.FirstName == expectedFirstName && x.LastName == expectedLastName);
           
            // Assert
            Assert.IsNotNull(contacts);
            Assert.That(contacts[2].firstName, Is.EqualTo(expectedFirstName));
            Assert.That(contacts[2].lastName, Is.EqualTo(expectedLastName));

            //Assert.That(contacts[2].firstName, Is.EqualTo("Albert"));
            //Assert.That(contacts[2].lastName, Is.EqualTo("Einstein"));
        }

        [Test]
        public void Test_SearchByKeyword_InvalidResults()
        {
            // Arrange
            var request = new RestRequest("contacts/search/missing" + DateTime.Now.Ticks, Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.EqualTo("[]"));
        }

        [Test]
        public void Test_TryToCreateContact_Missing_FirstName()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                lastName = "Curie",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend"
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"First name cannot be empty!\"}"));
        }

        [Test]
        public void Test_CreateContact_ValidBody()
        {
            // Arrange
            var request = new RestRequest("contacts", Method.Post);
            var reqBody = new
            {
                firstName = "Marie",
                lastName = "Curie",
                email = "marie67@gmail.com",
                phone = "+1 800 200 300",
                comments = "Old friend"
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);
            var contactObject = JsonSerializer.Deserialize<contactObject>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(contactObject.msg, Is.EqualTo("Contact added."));
            Assert.That(contactObject.contact.id, Is.GreaterThan(0));
            Assert.That(contactObject.contact.firstName, Is.EqualTo(reqBody.firstName));
            Assert.That(contactObject.contact.lastName, Is.EqualTo(reqBody.lastName));
            Assert.That(contactObject.contact.email, Is.EqualTo(reqBody.email));
            Assert.That(contactObject.contact.phone, Is.EqualTo(reqBody.phone));
            Assert.That(contactObject.contact.dateCreated, Is.Not.Empty);
            Assert.That(contactObject.contact.comments, Is.EqualTo(reqBody.comments));
        }
    }
}