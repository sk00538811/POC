using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/* 
   Licensed under the Apache License, Version 2.0

   http://www.apache.org/licenses/LICENSE-2.0
   */

namespace FileMetaData.Classes
{
    public class CustomerLegacy
    {
/*
        [XmlRoot(ElementName = "user1")]
        public class User1
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "user2")]
        public class User2
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "user3")]
        public class User3
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "titleDescription")]
        public class TitleDescription
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "address2")]
        public class Address2
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "address3")]
        public class Address3
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "address4")]
        public class Address4
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "address5")]
        public class Address5
        {
            [XmlAttribute(AttributeName = "null")]
            public string Null { get; set; }
        }

        [XmlRoot(ElementName = "address")]
        public class Address
        {
            [XmlElement(ElementName = "firstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "lastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "addressTypeId")]
            public string AddressTypeId { get; set; }
            [XmlElement(ElementName = "address1")]
            public string Address1 { get; set; }
            [XmlElement(ElementName = "address2")]
            public Address2 Address2 { get; set; }
            [XmlElement(ElementName = "address3")]
            public Address3 Address3 { get; set; }
            [XmlElement(ElementName = "address4")]
            public Address4 Address4 { get; set; }
            [XmlElement(ElementName = "address5")]
            public Address5 Address5 { get; set; }
            [XmlElement(ElementName = "city")]
            public string City { get; set; }
            [XmlElement(ElementName = "regionCode")]
            public string RegionCode { get; set; }
            [XmlElement(ElementName = "countryCode")]
            public string CountryCode { get; set; }
            [XmlElement(ElementName = "postCode")]
            public string PostCode { get; set; }
            [XmlElement(ElementName = "primary")]
            public string Primary { get; set; }
            [XmlElement(ElementName = "validAddress")]
            public string ValidAddress { get; set; }
            [XmlElement(ElementName = "readOnlyAccess")]
            public string ReadOnlyAccess { get; set; }
            [XmlElement(ElementName = "privateAccess")]
            public string PrivateAccess { get; set; }
            [XmlAttribute(AttributeName = "action")]
            public string Action { get; set; }
        }

        [XmlRoot(ElementName = "addresses")]
        public class Addresses
        {
            [XmlElement(ElementName = "address")]
            public Address Address { get; set; }
        }

        [XmlRoot(ElementName = "EmailContact")]
        public class EmailContact
        {
            [XmlElement(ElementName = "id")]
            public string Id { get; set; }
            [XmlElement(ElementName = "contactId")]
            public string ContactId { get; set; }
            [XmlElement(ElementName = "secondaryId")]
            public string SecondaryId { get; set; }
            [XmlElement(ElementName = "title")]
            public string Title { get; set; }
            [XmlElement(ElementName = "firstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "lastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "email_address")]
            public string Email_address { get; set; }
            [XmlElement(ElementName = "phoneNumber")]
            public string PhoneNumber { get; set; }
            [XmlElement(ElementName = "accountName")]
            public string AccountName { get; set; }
            [XmlElement(ElementName = "departmentName")]
            public string DepartmentName { get; set; }
            [XmlElement(ElementName = "raId")]
            public string RaId { get; set; }
            [XmlElement(ElementName = "deptId")]
            public string DeptId { get; set; }
            [XmlElement(ElementName = "schoolId")]
            public string SchoolId { get; set; }
            [XmlElement(ElementName = "primary")]
            public string Primary { get; set; }
            [XmlElement(ElementName = "invalid")]
            public string Invalid { get; set; }
            [XmlElement(ElementName = "subType")]
            public string SubType { get; set; }
            [XmlElement(ElementName = "contactStatusId")]
            public string ContactStatusId { get; set; }
            [XmlElement(ElementName = "contactStatus")]
            public string ContactStatus { get; set; }
            [XmlElement(ElementName = "createdDate")]
            public string CreatedDate { get; set; }
            [XmlElement(ElementName = "schoolType")]
            public string SchoolType { get; set; }
            [XmlElement(ElementName = "contactPreference")]
            public string ContactPreference { get; set; }
            [XmlElement(ElementName = "private")]
            public string Private { get; set; }
        }

        [XmlRoot(ElementName = "EmailContacts")]
        public class EmailContacts
        {
            [XmlElement(ElementName = "EmailContact")]
            public EmailContact EmailContact { get; set; }
        }
        */
        [XmlRoot(ElementName = "individual")]
        public class Individual
        {
           /* [XmlElement(ElementName = "secondaryId")]
            public string SecondaryId { get; set; }
            [XmlElement(ElementName = "firstName")]
            public string FirstName { get; set; }
            [XmlElement(ElementName = "lastName")]
            public string LastName { get; set; }
            [XmlElement(ElementName = "emailAddress")]
            public string EmailAddress { get; set; }
            [XmlElement(ElementName = "companyId")]
            public string CompanyId { get; set; }
            [XmlElement(ElementName = "companyName")]
            public string CompanyName { get; set; }
            [XmlElement(ElementName = "companySecId")]
            public string CompanySecId { get; set; }
            [XmlElement(ElementName = "companySfId")]
            public string CompanySfId { get; set; }*/
            [XmlElement(ElementName = "assignedId")]
            public string AssignedId { get; set; }
           /* [XmlElement(ElementName = "cAcct")]
            public string CAcct { get; set; }
            [XmlElement(ElementName = "contactStatus")]
            public string ContactStatus { get; set; }
            [XmlElement(ElementName = "recordType")]
            public string RecordType { get; set; }
            [XmlElement(ElementName = "statusId")]
            public string StatusId { get; set; }
            [XmlElement(ElementName = "userTypeId")]
            public string UserTypeId { get; set; }
            [XmlElement(ElementName = "userSubTypeId")]
            public string UserSubTypeId { get; set; }
            [XmlElement(ElementName = "user1")]
            public User1 User1 { get; set; }
            [XmlElement(ElementName = "user2")]
            public User2 User2 { get; set; }
            [XmlElement(ElementName = "user3")]
            public User3 User3 { get; set; }
            [XmlElement(ElementName = "user6")]
            public string User6 { get; set; }
            [XmlElement(ElementName = "user8")]
            public string User8 { get; set; }
            [XmlElement(ElementName = "titleDescription")]
            public TitleDescription TitleDescription { get; set; }
            [XmlElement(ElementName = "primaryAddressType")]
            public string PrimaryAddressType { get; set; }
            [XmlElement(ElementName = "addresses")]
            public Addresses Addresses { get; set; }
            [XmlElement(ElementName = "phones")]
            public string Phones { get; set; }
            [XmlElement(ElementName = "EmailContacts")]
            public EmailContacts EmailContacts { get; set; }*/
            [XmlAttribute(AttributeName = "action")]
            public string Action { get; set; }
        }

    }
}