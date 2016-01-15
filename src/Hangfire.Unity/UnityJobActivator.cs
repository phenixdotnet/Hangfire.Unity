using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire
{
    /// <summary>
    /// A job activator which use unity to create job instance
    /// </summary>
    public class UnityJobActivator : JobActivator
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initialize a new instance of the <see cref="T:UnityJobActivator"/> class
        /// </summary>
        /// <param name="container">The unity container to be used</param>
        public UnityJobActivator(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        /// <inheritdoc />
        public override object ActivateJob(Type jobType)
        {
            
            return this.container.Resolve(jobType);
        }
    }
}
