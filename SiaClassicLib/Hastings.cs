using Math.Gmp.Native;
using System;

namespace SiaClassicLib
{
    public class Hastings : IDisposable
    {
        static readonly mpf_t d = getd();

        static mpf_t getd()
        {
            mpf_t d = new mpf_t();
            gmp_lib.mpf_init(d);
            char_ptr value = new char_ptr("1000000000000000000000000");
            gmp_lib.mpf_set_str(d, value, 10);
            return d;
        }

        mpf_t value = new mpf_t();

        public Hastings() { gmp_lib.mpf_init(value); }

        public void Add(string hastings)
        {
            var s = new char_ptr(hastings);
            mpf_t x = new mpf_t();
            gmp_lib.mpf_init(x);
            gmp_lib.mpf_set_str(x, s, 10);
            gmp_lib.free(s);
            gmp_lib.mpf_div(x, x, d);
            gmp_lib.mpf_add(value, value, x);
            gmp_lib.mpf_clear(x);
        }

        public string ToString(string format = "%Ff")
        {
            ptr<char_ptr> str = new ptr<char_ptr>();
            gmp_lib.gmp_asprintf(str, format, value);
            var res = str.Value.ToString();
            gmp_lib.free(str.Value);
            return res;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                gmp_lib.mpf_clear(value);
                value = null;

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Hastings()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
