using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishToClassDefinition.Models
{
    /// <summary>
    /// Request model for creating carts.
    /// </summary>
    /// <remarks>
    /// <div class="bs-callout bs-callout-info"><strong>Note:</strong> At least one item must be specified in either
    /// the <em>ItemsByLicenseKey</em> or the <em>ItemsByAttributes</em> collection.</div>
    /// </remarks>
    public class CreateCartRequestServiceModel
    {
        /// <summary>
        /// The type of order to be placed ("RESELLER", "MSP").
        /// </summary>
        [Required]
        public string TypeOfOrder { get; set; }

        /// <summary>
        /// The language and territory to use as the locale setting for the order. The format of the 
        /// locale must be the ISO two-letter language code (lowercase) and the ISO two-letter country code (UPPERCASE),
        /// concatenated with an underscore ('_') character. E.g. en_US, ja_JP.
        /// <p>The locales that can be used are defined by the partner account configuration. Please reach out to your 
        /// Webroot e-commerce business partner to identify valid locales for your orders.</p>
        /// </summary>
        [Required]
        public string Locale { get; set; }

        /// <summary>
        /// Items to be added to the cart based on license keys.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1819:Properties should not return arrays", Justification = "Legacy code")]
        public CreateCartRequestServiceModel_ItemByLicenseKey[] ItemsByLicenseKey { get; set; }

        /// <summary>
        /// Items to be added to the cart based on attributes.
        /// </summary>
        public CreateCartRequestServiceModel_ItemByAttributes[] ItemsByAttributes { get; set; }


        /// <summary>
        /// Creates a new instance of the class that implements the ISampleModelGenerator interface with
        /// its properties and fields set to example values.
        /// </summary>
        /// <param name="collectionSize">The number of objects to put into collections.</param>
        /// <returns>Sample class object.</returns>
        public object GenerateSample(int collectionSize)
        {
            CreateCartRequestServiceModel sample = new CreateCartRequestServiceModel()
            {
                TypeOfOrder = "MSP",
                Locale = "en_US"
            };

            sample.ItemsByAttributes = new CreateCartRequestServiceModel_ItemByAttributes[2];

            sample.ItemsByAttributes[0] = new CreateCartRequestServiceModel_ItemByAttributes()
            {
                LicenseCategoryName = "SAEP",
                LicenseSeats = 150,
                Years = "1",
                LicenseKeycodeTypeId = 3,
                LicenseAttributeValue = 110,
                ItemHierarchyId = 1,
                CartItemBundleId = 1
            };

            sample.ItemsByAttributes[1] = new CreateCartRequestServiceModel_ItemByAttributes()
            {
                LicenseCategoryName = "SDNS",
                LicenseSeats = 150,
                Years = "1",
                LicenseKeycodeTypeId = 3,
                LicenseAttributeValue = 110,
                ItemHierarchyId = 2,
                CartItemBundleId = 1
            };

            return sample;
        }


    }


    /// <summary>
    /// Base class for items in CreateCartRequestServiceModel.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Legacy naming")]
    public class CreateCartRequestServiceModel_Item
    {
        /// <summary>
        /// The number of years the license should be valid.
        /// <p>Possible values are: 0, 0.083, 0.167, 0.5, 1, 1.25, 1.5, 2, 2.25, 2.5, 3, 3.25, 3.5.</p>
        /// </summary>
        [Required]
        public string Years { get; set; }

        /// <summary>
        /// The identifier of a license's keycode type. 
        /// <p>Possible values are: 1 = standard license, 3 = parent license. If not specified, a default of 1 is assumed.</p>
        /// </summary>
        public int? LicenseKeycodeTypeId { get; set; }

        /// <summary>
        /// An identifier specifying whether the cart item is a primary or secondary product.
        /// Secondary products have an <em>ItemHierarchyId</em> of 2
        /// and represent products dependent on a primary product.
        /// Examples include SDNS (Webroot DNS Protection), SECA (Webroot Security Awareness Training), OTEDR (OpenText Endpoint Detection and Response), OTMDR (OpenText Managed Detection and Response) and
        /// OSBPB (OpenText Server Backup Public Cloud - Bring Your Own Cloud) products provisioned as part of an item whose primary product is SAEP.
        /// <p>Note that the <em>ItemHierarchyId</em> field cannot be used without a <em>CartItemBundleId</em>.</p>
        /// If not specified, a default of 1 is assumed.
        /// </summary>
        public int? ItemHierarchyId { get; set; }

        /// <summary>
        /// The expiration date of this license. Although <em>Years</em> is a required input for the cart item,
        /// it is possible to define a custom expiration date by using this parameter.
        /// Specifying a custom expiration date needs to be enabled on your partner account.
        /// Please contact your Webroot representative, if you require this functionality.
        /// </summary>
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// The vendor specific order item code. 
        /// </summary>
        public string VendorOrderItemCode { get; set; }

        /// <summary>
        /// The bundle identifier for this item.
        /// Defines whether the items specified in the cart belong to the same license/keycode.
        /// If multiple items are associated with the same bundle, only one of them can have an <em>ItemHierarchyId</em> of 1.
        /// </summary>
        public int? CartItemBundleId { get; set; }

        /// <summary>
        /// A value which indicates the product usage model for this item.
        /// Digit one defines the order type (1 = MSP, 2 = reseller).
        /// Digit two determines which terms a license is based on (1 = monthly, 2 = annually).
        /// Digit three refers to the associated billing model (0 = sold, 1 = overage, 2 = utility).
        /// As an example, 210 stands for "Reseller Monthly Sold". 
        /// Please note that the exact values you can use are determined by your contract with Webroot.
        /// </summary>
        public int? LicenseAttributeValue { get; set; }

        /// <summary>
        /// A value which indicates how usage will be calculated for this item.
        /// Only applicable to certain license categories like OTSF (1 = Advanced, 2 = Capacity) and CBEP (1 = Advanced, 3 = Standard).
        /// If not specified, a default is assumed.
        /// Please speak with your Webroot e-commerce business partner for defaults/values specific to your contract.
        /// </summary>
        public int? UsagePricingModelId { get; set; }

        /// <summary>
        /// Defines which data center vault to use (applicable to OTSF, CBEP, OTEDR and OTMDR). 
        /// Ids are associated with license categories. As an example, 
        /// a value of '1' defines a data center in US2 for CBEP.
        /// <p><strong>Note:</strong> For the license categories OTEDR, OTMDR and CALLY the field <em>VaultId</em> is mandatory.</p>
        /// Supported values are:
        /// <ul>
        /// <li> CBEP: </li>
        /// <ul>
        /// <li> 18 = US </li>
        /// <li> 1 = US2 </li>
        /// <li> 2 = EMEA </li>
        /// <li> 3 = CA </li>
        /// <li> 4 = FR </li>
        /// <li> 5 = APAC </li>
        /// <li> 15 = AU </li>
        /// <li> 16 = UK </li>
        /// <li> 104 = IN </li>
        /// </ul>
        /// <li> OTSF </li>
        /// <ul>
        /// <li> 8 = East US 2 (Virginia) </li>
        /// <li> 9 = Australia East (New South Wales) </li>
        /// <li> 10 = Canada Central (Toronto) </li>
        /// <li> 11 = France Central (Paris) </li>
        /// <li> 12 = UK South (London) </li>
        /// <li> 103 = India </li>
        /// <li> 130 = O365 MSP Generic Vault </li>
        /// </ul>
        /// <li> OTEDR/OTMDR </li>
        /// <ul>
        /// <li> 131 = US1 </li>
        /// <li> 132 = UK1 </li>
        /// </ul>
        /// <li> CALLY </li>
        /// <ul>
        /// <li> 133 = US_EST </li>
        /// <li> 134 = EU </li>
        /// <li> 135 = EU_FRANKFURT </li>
        /// <li> 136 = CANADA </li>
        /// <li> 137 = ASIA_SIDNEY </li>
        /// <li> 138 = ASIA_TOKYO </li>
        /// <li> 139 = LONDON </li>
        /// <li> 140 = EU_PARIS </li>
        /// <li> 141 = AFRICA_CAPE_TOWN </li>
        /// <li> 142 = ASIA_MUMBAI </li>
        /// <li> 143 = EU_ZURICH </li>
        /// </ul>
        /// </ul>
        /// </summary>
        public int? VaultId { get; set; }

        /// <summary>
        /// Platform to use for this item (applicable to CBEP only).
        /// If nothing is provided, a default value of 1 is assumed.
        /// Currently supported values are:
        /// <ul>
        /// <li> 1 = Carbonite. Valid for <em>LicenseKeycodeTypeId</em> 3 and 1. </li>
        /// <li> 2 = Azure EA. Valid for Business Single Site in Reseller Cart (<em>LicenseKeycodeTypeId</em>=1). </li>
        /// <li> 3 = OnPrem. Valid for Business Single Site in Reseller Cart (<em>LicenseKeycodeTypeId</em>=1). </li>
        /// </ul>
        /// </summary>
        public int? ProductPlatformId { get; set; }

        /// <summary>
        /// Number of years data is retained (applicable to OTSF only).
        /// Legitimate values are 1 (for one year) or 2 (for seven years).
        /// Default is 1.
        /// </summary>
        public int? RetentionModelId { get; set; }

        /// <summary>
        /// Pricing level to use for this item
        /// (1 = Standard, 2 = EDU/Non Profit, 3 = Government).
        /// Only valid for resellers (not for MSP orders). Partners who are not configured will
        /// get a 'no pricing found' message.
        /// </summary>
        public int? ProductPricingLevelId { get; set; }
    }


    /// <summary>
    /// Element for cart items based on license keys.
    /// </summary>
    /// <remarks>
    /// <p><div class="bs-callout bs-callout-info"><strong>NOTE:</strong> Each partner account configuration 
    /// can have different products set up (e.g. WIFI product for 1 year with 10 seats), which restrict 
    /// the values that can be specified for the <em>LicenseCategoryName</em>, <em>LicenseSeats</em>, 
    /// and <em>Years</em> fields. Please work with your Webroot e-commerce business partner to identify valid
    /// order combinations.</div></p>
    /// </remarks>
    public class CreateCartRequestServiceModel_ItemByLicenseKey : CreateCartRequestServiceModel_Item
    {
        /// <summary>
        /// The number of seats the license should have.
        /// </summary>
        // Not required in this case.
        public int? LicenseSeats { get; set; }

        /// <summary>
        /// License category associated to this item.
        /// <p>Possible values are for example: WIFI, OTSF, CBEP, WSAV, WSAI, WSAC, SAEP, SDNS, SECA, OTEDR, OTMDR, PLRP, CALLY, OSBPB, OTAES.</p>
        /// </summary>
        // Not required in this case.
        public string LicenseCategoryName { get; set; }

        /// <summary>
        /// The license key with which to create either a renewal or an upgrade item.
        /// </summary>
        [Required]
        public string Keycode { get; set; }
    }


    /// <summary>
    /// Element for cart items based on attributes.
    /// </summary>
    /// <remarks>
    /// <p><div class="bs-callout bs-callout-info"><strong>NOTE:</strong> Each partner account configuration 
    /// can have different products set up (e.g. WIFI product for 1 year with 10 seats), which restrict 
    /// the values that can be specified for the <em>LicenseCategoryName</em>, <em>LicenseSeats</em>, 
    /// and <em>Years</em> fields. Please work with your Webroot e-commerce business partner to identify valid
    /// order combinations.</div></p>
    /// </remarks>
   
    public class CreateCartRequestServiceModel_ItemByAttributes : CreateCartRequestServiceModel_Item
    {
        /// <summary>
        /// The number of seats the license should have.
        /// </summary>
        [Required]
        public int LicenseSeats { get; set; }

        /// <summary>
        /// License category associated to this item.
        /// <p>Possible values are for example: WIFI, OTSF, CBEP, WSAV, WSAI, WSAC, SAEP, SDNS, SECA, OTEDR, OTMDR, PLRP, CALLY, OSBPB, OTAES.</p>
        /// </summary>
        [Required]
        public string LicenseCategoryName { get; set; }
    }
}
