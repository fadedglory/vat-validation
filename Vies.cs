using System.ComponentModel;
using System.ServiceModel;

namespace vat.validation
{
    [ServiceContract(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat", ConfigurationName = "Vies.checkVatPortType")]
    public interface CheckVatPortType
    {
        [OperationContract(Action = "", ReplyAction = "*")]
        CheckVatResponse CheckVat(CheckVatRequest request);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "CheckVat", WrapperNamespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", IsWrapped = true)]
    public class CheckVatRequest
    {
        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 0)] public string countryCode;

        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 1)] public string vatNumber;

        public CheckVatRequest()
        {
        }

        public CheckVatRequest(string countryCode, string vatNumber)
        {
            this.countryCode = countryCode;
            this.vatNumber = vatNumber;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "checkVatResponse", WrapperNamespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", IsWrapped = true)]
    public class CheckVatResponse
    {
        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 5)] public string address;

        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 0)] public string countryCode;

        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 4)] public string name;

        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 2)] public string requestDate;

        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 3)] public bool valid;

        [MessageBodyMember(Namespace = "urn:ec.europa.eu:taxud:vies:services:CheckVat:types", Order = 1)] public string vatNumber;

        public CheckVatResponse()
        {
        }

        public CheckVatResponse(string countryCode, string vatNumber, string requestDate, bool valid, string name, string address)
        {
            this.countryCode = countryCode;
            this.vatNumber = vatNumber;
            this.requestDate = requestDate;
            this.valid = valid;
            this.name = name;
            this.address = address;
        }
    }

    public interface CheckVatPortTypeChannel : CheckVatPortType, IClientChannel
    {
    }

    public class CheckVatPortTypeClient : ClientBase<CheckVatPortType>, CheckVatPortType
    {
        public CheckVatPortTypeClient() : base(new BasicHttpBinding(BasicHttpSecurityMode.None),
                                               new EndpointAddress("http://ec.europa.eu/taxation_customs/vies/services/checkVatService"))
        {
        }


        #region CheckVatPortType Members

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        CheckVatResponse CheckVatPortType.CheckVat(CheckVatRequest request)
        {
            return Channel.CheckVat(request);
        }

        #endregion

    }
}