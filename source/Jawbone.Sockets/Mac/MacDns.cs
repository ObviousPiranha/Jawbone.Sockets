using System.Collections;
using System.Collections.Generic;

namespace Jawbone.Sockets.Mac;

sealed class MacDns
{
    public sealed class Enumerable : IEnumerable<IpEndpoint>
    {
        private readonly string? _node;
        private readonly string? _service;
        private readonly IpAddressVersion _filter;

        public Enumerable(string? node, string? service, IpAddressVersion filter)
        {
            _node = node;
            _service = service;
            _filter = filter;
        }

        public IEnumerator<IpEndpoint> GetEnumerator() => new Enumerator(_node, _service, _filter);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public sealed unsafe class Enumerator : IEnumerator<IpEndpoint>
    {
        private readonly AddrInfo* _addrInfo;
        private AddrInfo* _nextInfo;
        private readonly IpAddressVersion _filter;

        public Enumerator(string? node, string? service, IpAddressVersion filter)
        {
            var hints = default(AddrInfo);
            hints.AiFamily = filter switch
            {
                IpAddressVersion.V4 => Af.INet,
                IpAddressVersion.V6 => Af.INet6,
                _ => Af.Unspec
            };
            var result = Sys.GetAddrInfo(node, service, hints, out _addrInfo);

            if (result == -1)
                Sys.Throw(ExceptionMessages.Dns);

            _nextInfo = _addrInfo;
            _filter = filter;
        }

        public IpEndpoint Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            Sys.FreeAddrInfo(_addrInfo);
        }

        public bool MoveNext()
        {
            while (_nextInfo != null)
            {
                var ai = _nextInfo;
                _nextInfo = _nextInfo->AiNext;

                if (ai->AiAddr == null)
                    continue;
                if (_filter != IpAddressVersion.V6 && ai->AiFamily == Af.INet)
                {
                    var addr = (SockAddrIn*)ai->AiAddr;
                    Current = addr->ToEndpoint();
                    return true;
                }
                if (_filter != IpAddressVersion.V4 && ai->AiFamily == Af.INet6)
                {
                    var addr = (SockAddrIn6*)ai->AiAddr;
                    Current = addr->ToEndpoint();
                    return true;
                }
            }

            return false;
        }

        public void Reset() => _nextInfo = _addrInfo;
    }
}
