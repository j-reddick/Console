﻿using System.Collections;
using System.Data;
using System.Linq;
using System.Management.Automation;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Text;

namespace Cognifide.PowerShell.PowerShellIntegrations.Commandlets.Presentation
{
    [Cmdlet(VerbsCommon.Add, "Rendering")]
    public class AddRenderingCommand : BaseLayoutCommand
    {
        private int index = -1;

        [Parameter(Mandatory = true)]
        [Alias("Rendering")]
        public RenderingDefinition Instance { get; set; }

        [Parameter]
        public Hashtable Parameter { get; set; }

        [Parameter(Mandatory = true)]
        public string PlaceHolder { get; set; }

        [Parameter]
        public Item DataSource { get; set; }

        [Parameter]
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        protected override void ProcessLayout(Item item, LayoutDefinition layout, DeviceDefinition device)
        {
            var rendering = new RenderingDefinition
            {
                ItemID = Instance.ItemID,
                Placeholder = PlaceHolder,
                Datasource = DataSource != null ? DataSource.ID.ToString() : Instance.Datasource,
                Cachable = Instance.Cachable,
                VaryByData = Instance.VaryByData,
                VaryByDevice = Instance.VaryByDevice,
                VaryByLogin = Instance.VaryByLogin,
                VaryByParameters = Instance.VaryByParameters,
                VaryByQueryString = Instance.VaryByQueryString,
                VaryByUser = Instance.VaryByUser,
            };

            if (Parameter != null)            
            {
                var parameters = new UrlString(rendering.Parameters ?? string.Empty);
                foreach (string name in Parameter.Keys)
                    if (parameters.Parameters.AllKeys.Contains(name))
                    {
                        parameters.Parameters[name] = Parameter[name].ToString();
                    }
                    else
                    {
                        parameters.Add(name, Parameter[name].ToString());                        
                    }
                rendering.Parameters = parameters.ToString();
            }

            //todo: add support for conditions
            //renderingDefinition.Conditions
            //todo: add support for multivariate tests
            //rendering.MultiVariateTest

            if (Index > -1)
            {
                device.Insert(index, rendering);
            }
            else
            {
                device.AddRendering(rendering);
            }

            item.Edit(p =>
            {
                string outputXml = layout.ToXml();
                Item["__Renderings"] = outputXml;
            });
        }
    }
}