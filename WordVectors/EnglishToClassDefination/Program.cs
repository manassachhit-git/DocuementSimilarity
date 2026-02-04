using EnglishToClassDefinition.Models;
using EnglishToClassDefinition.Service;
using EnglishToClassDefinition.Utilities;

namespace EnglishToClassDefinition
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //var ai = new LamaAIConverterService(
            //endpointUrl: "api endpoint",
            //apiKey: "api key");


            var ai = new SelfCorrectorLLamaService("api key",
            "api endpoint"
            );

            var converter = new Utilities.EnglishToJsonConverterUtility(ai);

            //var refactoredConverter = new Utilities.ConverterUtility(null, ai);

            //string englishText = "Create an employee profile:\r\nId=101, Amit Sharma, FullTime.\r\nJoined 2020-05-12, promoted 2023-01-10, earning 85000.\r\n\r\nHomeAddress:\r\nStreet Bagmane Techpark, Bangalore, India, ZIP 560037.\r\n\r\nEmergency contacts:\r\nRohan (Brother) 9998887771\r\nAnjali (Mother) 8887776665 likes reading and playing cricket.";
            //string json = refactorConverter.ConvertAsyncWithSelfCorrection<Employee>(englishText).Result;
            //Console.WriteLine(json);

            //string ComplexEmployeeText = "Employee Priya Verma (Id: 102) is a FullTime Senior Software Engineer earning 292000.\r\nJoined on 2019-07-01 and promoted on 2022-09-15.\r\n\r\nAddress: Bagmane Techpark, Bangalore, India, 560037.\r\n\r\nContacts:\r\n• Raj (Father) – 9988776655\r\n• Kavya (Sister) – 8877665544\r\n\r\nSkilled in: Java, Microservices, System Design\r\nHobbies: Trekking, Photography\r\n\r\nProjects:\r\n• Accounts Revamp – Active since 2023-01-10\r\n• Payment Gateway Upgrade – 2021-03-05 to 2022-11-10\r\n\r\nAdditional Attributes:\r\nWorked on: 5 Years\r\nCertifications: AWS, Azure\r\nSpeaks: English, Hindi\r\n\r\nRole Details: Is a full stack developer uses Java";
            //string ComplexJson = converter.ConvertAsyncWithSelfCorrection<ComplexEmployee>(ComplexEmployeeText).Result;
            //Console.WriteLine(ComplexJson);


            string CreateCartText = "Create a new cart for an MSP order with the locale set to\"en_US\".Add two items to the cart based on attributes:1.The first item is for the\"SAEP\"license category,with 150 seats,valid for 1 year.• License Keycode Type:Parent license(ID 3)\r\n• Item Hierarchy:Primary product(ID 1)\r\n• Bundle ID:1\r\n• License Attribute Value:110\r\n2.The second item is for the\"SDNS\"license category,also with 150 seats,valid for 1 year.• License Keycode Type:Parent license(ID 3)\r\n• Item Hierarchy:Secondary product(ID 2)\r\n• Bundle ID:1\r\n• License Attribute Value:110";
            string CartJson = converter.ConvertAsyncWithSelfCorrection<CreateCartRequestServiceModel>(CreateCartText).Result;
            Console.WriteLine(CartJson);

        }
    }
}
