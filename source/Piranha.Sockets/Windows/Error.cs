using System.Collections.Frozen;
using System.Collections.Immutable;

namespace Piranha.Sockets.Windows;

static class Error
{
    public static ErrorCode GetErrorCode(int error)
    {
        if (ErrorCodeById.TryGetValue(error, out var windowsErrorCode))
            return windowsErrorCode;

        return new(error);
    }

    public static bool IsInterrupt(int error) => error == 10004;

    private static readonly ImmutableArray<ErrorCode> ErrorCodes =
    [
        new(10004, "WSAEINTR", "A blocking operation was interrupted by a call to WSACancelBlockingCall."),
        new(10009, "WSAEBADF", "The file handle supplied is not valid."),
        new(10013, "WSAEACCES", "An attempt was made to access a socket in a way forbidden by its access permissions."),
        new(10014, "WSAEFAULT", "The system detected an invalid pointer address in attempting to use a pointer argument in a call."),
        new(10022, "WSAEINVAL", "An invalid argument was supplied."),
        new(10024, "WSAEMFILE", "Too many open sockets."),
        new(10035, "WSAEWOULDBLOCK", "A non-blocking socket operation could not be completed immediately."),
        new(10036, "WSAEINPROGRESS", "A blocking operation is currently executing."),
        new(10037, "WSAEALREADY", "An operation was attempted on a non-blocking socket that already had an operation in progress."),
        new(10038, "WSAENOTSOCK", "An operation was attempted on something that is not a socket."),
        new(10039, "WSAEDESTADDRREQ", "A required address was omitted from an operation on a socket."),
        new(10040, "WSAEMSGSIZE", "A message sent on a datagram socket was larger than the internal message buffer or some other network limit, or the buffer used to receive a datagram into was smaller than the datagram itself."),
        new(10041, "WSAEPROTOTYPE", "A protocol was specified in the socket function call that does not support the semantics of the socket type requested."),
        new(10042, "WSAENOPROTOOPT", "An unknown, invalid, or unsupported option or level was specified in a getsockopt or setsockopt call."),
        new(10043, "WSAEPROTONOSUPPORT", "The requested protocol has not been configured into the system, or no implementation for it exists."),
        new(10044, "WSAESOCKTNOSUPPORT", "The support for the specified socket type does not exist in this address family."),
        new(10045, "WSAEOPNOTSUPP", "The attempted operation is not supported for the type of object referenced."),
        new(10046, "WSAEPFNOSUPPORT", "The protocol family has not been configured into the system or no implementation for it exists."),
        new(10047, "WSAEAFNOSUPPORT", "An address incompatible with the requested protocol was used."),
        new(10048, "WSAEADDRINUSE", "Only one usage of each socket address (protocol/network address/port) is normally permitted."),
        new(10049, "WSAEADDRNOTAVAIL", "The requested address is not valid in its context."),
        new(10050, "WSAENETDOWN", "A socket operation encountered a dead network."),
        new(10051, "WSAENETUNREACH", "A socket operation was attempted to an unreachable network."),
        new(10052, "WSAENETRESET", "The connection has been broken due to keep-alive activity detecting a failure while the operation was in progress."),
        new(10053, "WSAECONNABORTED", "An established connection was aborted by the software in your host machine."),
        new(10054, "WSAECONNRESET", "An existing connection was forcibly closed by the remote host."),
        new(10055, "WSAENOBUFS", "An operation on a socket could not be performed because the system lacked sufficient buffer space or because a queue was full."),
        new(10056, "WSAEISCONN", "A connect request was made on an already connected socket."),
        new(10057, "WSAENOTCONN", "A request to send or receive data was disallowed because the socket is not connected and (when sending on a datagram socket using a sendto call) no address was supplied."),
        new(10058, "WSAESHUTDOWN", "A request to send or receive data was disallowed because the socket had already been shut down in that direction with a previous shutdown call."),
        new(10059, "WSAETOOMANYREFS", "Too many references to some kernel object."),
        new(10060, "WSAETIMEDOUT", "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond."),
        new(10061, "WSAECONNREFUSED", "No connection could be made because the target machine actively refused it."),
        new(10062, "WSAELOOP", "Cannot translate name."),
        new(10063, "WSAENAMETOOLONG", "Name component or name was too long."),
        new(10064, "WSAEHOSTDOWN", "A socket operation failed because the destination host was down."),
        new(10065, "WSAEHOSTUNREACH", "A socket operation was attempted to an unreachable host."),
        new(10066, "WSAENOTEMPTY", "Cannot remove a directory that is not empty."),
        new(10067, "WSAEPROCLIM", "A Windows Sockets implementation may have a limit on the number of applications that may use it simultaneously."),
        new(10068, "WSAEUSERS", "Ran out of quota."),
        new(10069, "WSAEDQUOT", "Ran out of disk quota."),
        new(10070, "WSAESTALE", "File handle reference is no longer available."),
        new(10071, "WSAEREMOTE", "Item is not available locally."),
        new(10091, "WSASYSNOTREADY", "WSAStartup cannot function at this time because the underlying system it uses to provide network services is currently unavailable."),
        new(10092, "WSAVERNOTSUPPORTED", "The Windows Sockets version requested is not supported."),
        new(10093, "WSANOTINITIALISED", "Either the application has not called WSAStartup, or WSAStartup failed."),
        new(10101, "WSAEDISCON", "Returned by WSARecv or WSARecvFrom to indicate the remote party has initiated a graceful shutdown sequence."),
        new(10102, "WSAENOMORE", "No more results can be returned by WSALookupServiceNext."),
        new(10103, "WSAECANCELLED", "A call to WSALookupServiceEnd was made while this call was still processing. The call has been canceled."),
        new(10104, "WSAEINVALIDPROCTABLE", "The procedure call table is invalid."),
        new(10105, "WSAEINVALIDPROVIDER", "The requested service provider is invalid."),
        new(10106, "WSAEPROVIDERFAILEDINIT", "The requested service provider could not be loaded or initialized."),
        new(10107, "WSASYSCALLFAILURE", "A system call has failed."),
        new(10108, "WSASERVICE_NOT_FOUND", "No such service is known. The service cannot be found in the specified name space."),
        new(10109, "WSATYPE_NOT_FOUND", "The specified class was not found."),
        new(10110, "WSA_E_NO_MORE", "No more results can be returned by WSALookupServiceNext."),
        new(10111, "WSA_E_CANCELLED", "A call to WSALookupServiceEnd was made while this call was still processing. The call has been canceled."),
        new(10112, "WSAEREFUSED", "A database query failed because it was actively refused."),
        new(11001, "WSAHOST_NOT_FOUND", "No such host is known."),
        new(11002, "WSATRY_AGAIN", "This is usually a temporary error during hostname resolution and means that the local server did not receive a response from an authoritative server."),
        new(11003, "WSANO_RECOVERY", "A non-recoverable error occurred during a database lookup."),
        new(11004, "WSANO_DATA", "The requested name is valid, but no data of the requested type was found."),
        new(11005, "WSA_QOS_RECEIVERS", "At least one reserve has arrived."),
        new(11006, "WSA_QOS_SENDERS", "At least one path has arrived."),
        new(11007, "WSA_QOS_NO_SENDERS", "There are no senders."),
        new(11008, "WSA_QOS_NO_RECEIVERS", "There are no receivers."),
        new(11009, "WSA_QOS_REQUEST_CONFIRMED", "Reserve has been confirmed."),
        new(11010, "WSA_QOS_ADMISSION_FAILURE", "Error due to lack of resources."),
        new(11011, "WSA_QOS_POLICY_FAILURE", "Rejected for administrative reasons - bad credentials."),
        new(11012, "WSA_QOS_BAD_STYLE", "Unknown or conflicting style."),
        new(11013, "WSA_QOS_BAD_OBJECT", "Problem with some part of the filterspec or providerspecific buffer in general."),
        new(11014, "WSA_QOS_TRAFFIC_CTRL_ERROR", "Problem with some part of the flowspec."),
        new(11015, "WSA_QOS_GENERIC_ERROR", "General QOS error."),
        new(11016, "WSA_QOS_ESERVICETYPE", "An invalid or unrecognized service type was found in the flowspec."),
        new(11017, "WSA_QOS_EFLOWSPEC", "An invalid or inconsistent flowspec was found in the QOS structure."),
        new(11018, "WSA_QOS_EPROVSPECBUF", "Invalid QOS provider-specific buffer."),
        new(11019, "WSA_QOS_EFILTERSTYLE", "An invalid QOS filter style was used."),
        new(11020, "WSA_QOS_EFILTERTYPE", "An invalid QOS filter type was used."),
        new(11021, "WSA_QOS_EFILTERCOUNT", "An incorrect number of QOS FILTERSPECs were specified in the FLOWDESCRIPTOR."),
        new(11022, "WSA_QOS_EOBJLENGTH", "An object with an invalid ObjectLength field was specified in the QOS provider-specific buffer."),
        new(11023, "WSA_QOS_EFLOWCOUNT", "An incorrect number of flow descriptors was specified in the QOS structure."),
        new(11024, "WSA_QOS_EUNKOWNPSOBJ", "An unrecognized object was found in the QOS provider-specific buffer."),
        new(11025, "WSA_QOS_EPOLICYOBJ", "An invalid policy object was found in the QOS provider-specific buffer."),
        new(11026, "WSA_QOS_EFLOWDESC", "An invalid QOS flow descriptor was found in the flow descriptor list."),
        new(11027, "WSA_QOS_EPSFLOWSPEC", "An invalid or inconsistent flowspec was found in the QOS provider specific buffer."),
        new(11028, "WSA_QOS_EPSFILTERSPEC", "An invalid FILTERSPEC was found in the QOS provider-specific buffer."),
        new(11029, "WSA_QOS_ESDMODEOBJ", "An invalid shape discard mode object was found in the QOS provider specific buffer."),
        new(11030, "WSA_QOS_ESHAPERATEOBJ", "An invalid shaping rate object was found in the QOS provider-specific buffer."),
        new(11031, "WSA_QOS_RESERVED_PETYPE", "A reserved policy element was found in the QOS provider-specific buffer."),
        new(11032, "WSA_SECURE_HOST_NOT_FOUND", "No such host is known securely."),
        new(11033, "WSA_IPSEC_NAME_POLICY_ERROR", "Name based IPSEC policy could not be added."),
    ];

    private static readonly FrozenDictionary<int, ErrorCode> ErrorCodeById = ErrorCodes.ToFrozenDictionary(v => v.Id);
}
