using AutoMapper;
using System.Collections.Generic;
using System.Reflection;

namespace PeBank.API.Tests
{
    public class TestBase
    {
        private static IMapper _mapper;
        public static IMapper Mapper
        {
            get
            {
                var assemblies = new List<Assembly> { Assembly.Load("PeBank.API.Features") };

                if (_mapper != null)
                {
                    return _mapper;
                }

                var mappingConfig = new MapperConfiguration(mc =>
                {
                    mc.AddProfiles(assemblies);
                });

                var mapper = mappingConfig.CreateMapper();

                _mapper = mapper;
                return _mapper;
            }
        }
    }
}
