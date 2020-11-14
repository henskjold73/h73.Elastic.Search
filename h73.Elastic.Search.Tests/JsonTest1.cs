using System;

namespace eSmart.Elastic.Search.Tests
{


    public class JsonTest1
    {
        public int took { get; set; }
        public bool timed_out { get; set; }
        public _Shards _shards { get; set; }
        public Hits hits { get; set; }
    }

    public class _Shards
    {
        public int total { get; set; }
        public int successful { get; set; }
        public int skipped { get; set; }
        public int failed { get; set; }
    }

    public class Hits
    {
        public int total { get; set; }
        public float max_score { get; set; }
        public Hit[] hits { get; set; }
    }

    public class Hit
    {
        public string _index { get; set; }
        public string _type { get; set; }
        public string _id { get; set; }
        public float _score { get; set; }
        public _Source _source { get; set; }
    }

    public class _Source
    {
        public int AmiMeterType { get; set; }
        public Meterinstallation MeterInstallation { get; set; }
        public int Id { get; set; }
        public bool P2P { get; set; }
        public int MeterInstallationId { get; set; }
        public string Name { get; set; }
        public object[] SimCards { get; set; }
        public object FacilityId { get; set; }
        public string Description { get; set; }
        public object SimCardId { get; set; }
        public string MeterId { get; set; }
        public DateTime InstallationDate { get; set; }
        public object ModemImei { get; set; }
        public int Status { get; set; }
        public object CorporateCode { get; set; }
        public int RetailGroup { get; set; }
        public object PurchasePrice { get; set; }
        public int CustomerGroup { get; set; }
        public DateTime InServiceDate { get; set; }
        public int LoadProfile { get; set; }
        public int AddressId { get; set; }
        public object EstimatedAnnualUsage { get; set; }
        public DateTime ValidFrom { get; set; }
        public Powerdistributioncontract[] PowerDistributionContracts { get; set; }
        public DateTime ValidTo { get; set; }
        public object[] EnergyProviderContracts { get; set; }
        public string DefaultTimeZoneId { get; set; }
        public object ConversionFactor { get; set; }
        public int ParticipantId { get; set; }
        public float StartValue { get; set; }
        public object[] AssetActivityRecords { get; set; }
        public object Antenna { get; set; }
        public Address Address { get; set; }
        public object AntennaId { get; set; }
        public object[] AssetOwners { get; set; }
        public object[] AssetRelationships { get; set; }
        public object[] AssetParentRelationships { get; set; }
        public object[] AssetTimeSeries { get; set; }
        public object[] CommunicationUnits { get; set; }
        public object Participant { get; set; }
        public Location Location { get; set; }
        public Assetworkflow[] AssetWorkFlow { get; set; }
        public object[] AlarmHandlings { get; set; }
        public Category[] Categories { get; set; }
        public int AssetModelId { get; set; }
        public Assetmodel AssetModel { get; set; }
        public DateTime CreatedTime { get; set; }
        public object ImportSource { get; set; }
        public string Note { get; set; }
        public object[] Configurations { get; set; }
        public DateTime ModifiedTime { get; set; }
        public object[] AssetCalculations { get; set; }
        public object[] TimeSeriesModels { get; set; }
        public object SerialNumber { get; set; }
        public object[] InLinks { get; set; }
        public object[] OutLinks { get; set; }
        public object[] Links { get; set; }
        public object[] Workorders { get; set; }
        public string AssetGuid { get; set; }
        public object[] Contracts { get; set; }
        public object Resource { get; set; }
        public object[] AssetSubContracts { get; set; }
        public object[] CaseOrders { get; set; }
        public object LabelGuid { get; set; }
        public long SDBMHash { get; set; }
        public bool ChildEventsActive { get; set; }
        public object LabelTextValue { get; set; }
    }

    public class Meterinstallation
    {
        public string EANCode { get; set; }
        public object InstallationId { get; set; }
        public int Id { get; set; }
        public float SystemVoltage { get; set; }
        public string Name { get; set; }
        public int MainFuseNumPhases { get; set; }
        public object Description { get; set; }
        public float MainFuseCurrent { get; set; }
        public DateTime InstallationDate { get; set; }
        public int OverloadProtectionNumPhases { get; set; }
        public object CorporateCode { get; set; }
        public float OverloadProtectionCurrent { get; set; }
        public object PurchasePrice { get; set; }
        public int ShortCircuitNumPhases { get; set; }
        public DateTime InServiceDate { get; set; }
        public float ShortCircuitCurrent { get; set; }
        public int AddressId { get; set; }
        public object[] Meters { get; set; }
        public DateTime ValidFrom { get; set; }
        public string MeterPointNumber { get; set; }
        public object ValidTo { get; set; }
        public object IndustryCode { get; set; }
        public string DefaultTimeZoneId { get; set; }
        public int InstallationType { get; set; }
        public int ParticipantId { get; set; }
        public object MeteringSiteClassificationId { get; set; }
        public object[] AssetActivityRecords { get; set; }
        public object MeteringSiteClassification { get; set; }
        public object Address { get; set; }
        public object[] AssetOwners { get; set; }
        public object[] AssetRelationships { get; set; }
        public object[] AssetParentRelationships { get; set; }
        public object[] AssetTimeSeries { get; set; }
        public object[] CommunicationUnits { get; set; }
        public object Participant { get; set; }
        public object Location { get; set; }
        public object[] AssetWorkFlow { get; set; }
        public object[] AlarmHandlings { get; set; }
        public object[] Categories { get; set; }
        public object AssetModelId { get; set; }
        public object AssetModel { get; set; }
        public DateTime CreatedTime { get; set; }
        public object ImportSource { get; set; }
        public object Note { get; set; }
        public object[] Configurations { get; set; }
        public DateTime ModifiedTime { get; set; }
        public object[] AssetCalculations { get; set; }
        public object[] TimeSeriesModels { get; set; }
        public object SerialNumber { get; set; }
        public object[] InLinks { get; set; }
        public object[] OutLinks { get; set; }
        public object[] Links { get; set; }
        public object[] Workorders { get; set; }
        public string AssetGuid { get; set; }
        public object[] Contracts { get; set; }
        public object Resource { get; set; }
        public object[] AssetSubContracts { get; set; }
        public object[] CaseOrders { get; set; }
        public object LabelGuid { get; set; }
        public long SDBMHash { get; set; }
        public bool ChildEventsActive { get; set; }
        public object LabelTextValue { get; set; }
    }

    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public int PostalCode { get; set; }
        public string City { get; set; }
        public string CountryId { get; set; }
        public DateTime ValidDate { get; set; }
        public string StreetNumberExtension { get; set; }
        public int ParticipantId { get; set; }
        public Legalentitydetail[] LegalEntityDetails { get; set; }
        public object[] Assets { get; set; }
        public object Participant { get; set; }
        public string CoAddress { get; set; }
        public string ExtraAddress { get; set; }
        public string Country { get; set; }
        public string StateOrProvince { get; set; }
        public string StateOrProvinceId { get; set; }
        public object Postbox { get; set; }
    }

    public class Legalentitydetail
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationNo { get; set; }
        public object Code { get; set; }
        public string Note { get; set; }
        public string ExternalId { get; set; }
        public object DateOfBirth { get; set; }
        public int ParticipantId { get; set; }
        public int AddressId { get; set; }
        public object Participant { get; set; }
        public Electronicaddress ElectronicAddress { get; set; }
        public int? ElectronicAddressId { get; set; }
        public object GlobalLocationNumber { get; set; }
    }

    public class Electronicaddress
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public object Password { get; set; }
        public object UserId { get; set; }
        public string ExternalId { get; set; }
        public object Phone { get; set; }
        public object Cell { get; set; }
        public object SipAddress { get; set; }
        public object TwitterUser { get; set; }
        public object FacebookUser { get; set; }
        public object[] ComUnitServiceConnections { get; set; }
        public object[] Participants { get; set; }
    }

    public class Location
    {
        public int AssetId { get; set; }
        public object Asset { get; set; }
        public int SpartialReference { get; set; }
        public string GeoBinary { get; set; }
        public string AssetGuid { get; set; }
    }

    public class Assetmodel
    {
        public int Id { get; set; }
        public object Description { get; set; }
        public object Note { get; set; }
        public string ExternalId { get; set; }
        public string TypeCode { get; set; }
        public string ModelCode { get; set; }
        public bool HasOffSwitch { get; set; }
        public float InitialValue { get; set; }
        public string Manufacturer { get; set; }
        public object ModelVersion { get; set; }
        public object AssetModelProperties { get; set; }
    }

    public class Powerdistributioncontract
    {
        public int MeterId { get; set; }
        public string Id { get; set; }
        public object Meter { get; set; }
        public string Name { get; set; }
        public int ParticipantId { get; set; }
        public int OwnerLegalEntityId { get; set; }
        public object Participant { get; set; }
        public Ownerlegalentity OwnerLegalEntity { get; set; }
        public int PartnerLegalEntityId { get; set; }
        public Partnerlegalentity PartnerLegalEntity { get; set; }
        public int BuyOrSale { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public DateTime? CancelTime { get; set; }
        public object ExternalContractId { get; set; }
        public object ProductId { get; set; }
        public object Product { get; set; }
        public object[] Assets { get; set; }
        public object Signed { get; set; }
    }

    public class Ownerlegalentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EntityType { get; set; }
        public string ExternalId { get; set; }
        public object[] LegalEntityRoles { get; set; }
    }

    public class Partnerlegalentity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int EntityType { get; set; }
        public string ExternalId { get; set; }
        public Legalentityrole[] LegalEntityRoles { get; set; }
    }

    public class Legalentityrole
    {
        public int LegalEntityId { get; set; }
        public object LegalEntity { get; set; }
        public int LegalEntityDetailId { get; set; }
        public Legalentitydetail1 LegalEntityDetail { get; set; }
        public int RoleType { get; set; }
        public DateTime ValidFrom { get; set; }
        public object ValidTo { get; set; }
    }

    public class Legalentitydetail1
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationNo { get; set; }
        public object Code { get; set; }
        public string Note { get; set; }
        public string ExternalId { get; set; }
        public object DateOfBirth { get; set; }
        public int ParticipantId { get; set; }
        public int AddressId { get; set; }
        public object Participant { get; set; }
        public Address1 Address { get; set; }
        public Electronicaddress2 ElectronicAddress { get; set; }
        public int? ElectronicAddressId { get; set; }
        public object GlobalLocationNumber { get; set; }
    }

    public class Address1
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string StreetNumber { get; set; }
        public int? PostalCode { get; set; }
        public string City { get; set; }
        public string CountryId { get; set; }
        public DateTime ValidDate { get; set; }
        public string StreetNumberExtension { get; set; }
        public int ParticipantId { get; set; }
        public Legalentitydetail2[] LegalEntityDetails { get; set; }
        public object[] Assets { get; set; }
        public object Participant { get; set; }
        public string CoAddress { get; set; }
        public string ExtraAddress { get; set; }
        public string Country { get; set; }
        public string StateOrProvince { get; set; }
        public string StateOrProvinceId { get; set; }
        public object Postbox { get; set; }
    }

    public class Legalentitydetail2
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OrganisationNo { get; set; }
        public object Code { get; set; }
        public string Note { get; set; }
        public string ExternalId { get; set; }
        public object DateOfBirth { get; set; }
        public int ParticipantId { get; set; }
        public int AddressId { get; set; }
        public object Participant { get; set; }
        public Electronicaddress1 ElectronicAddress { get; set; }
        public int? ElectronicAddressId { get; set; }
        public object GlobalLocationNumber { get; set; }
    }

    public class Electronicaddress1
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public object Password { get; set; }
        public object UserId { get; set; }
        public string ExternalId { get; set; }
        public object Phone { get; set; }
        public object Cell { get; set; }
        public object SipAddress { get; set; }
        public object TwitterUser { get; set; }
        public object FacebookUser { get; set; }
        public object[] ComUnitServiceConnections { get; set; }
        public object[] Participants { get; set; }
    }

    public class Electronicaddress2
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public object Password { get; set; }
        public object UserId { get; set; }
        public string ExternalId { get; set; }
        public object Phone { get; set; }
        public object Cell { get; set; }
        public object SipAddress { get; set; }
        public object TwitterUser { get; set; }
        public object FacebookUser { get; set; }
        public object[] ComUnitServiceConnections { get; set; }
        public object[] Participants { get; set; }
    }

    public class Assetworkflow
    {
        public object InstanceId { get; set; }
        public int AssetId { get; set; }
        public int WorkFlowId { get; set; }
        public string Remarks { get; set; }
        public DateTime ExecutedTime { get; set; }
        public object Asset { get; set; }
        public object Workflow { get; set; }
        public object ValidFromDateTime { get; set; }
        public object ValidToDateTime { get; set; }
        public bool ManuallySet { get; set; }
        public string AssetGuid { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public object Description { get; set; }
        public int SourceStatus { get; set; }
        public string Source { get; set; }
        public object Participant { get; set; }
        public int ParticipantId { get; set; }
        public object[] Assets { get; set; }
        public int CategoryType { get; set; }
    }


}