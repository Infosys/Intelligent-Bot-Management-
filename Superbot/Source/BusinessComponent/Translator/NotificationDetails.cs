/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator
{
    public class NotificationDetails
    {
        public BE.ObservationDetails ObservationDetailsDEtoBE(IE.ObservationDetails notificationTransactionDataIE)
        {
            BE.ObservationDetails notificationTransactionDataBE = new BE.ObservationDetails();
            notificationTransactionDataBE.ObservationId = notificationTransactionDataIE.ObservationId;
            notificationTransactionDataBE.ObservableName = notificationTransactionDataIE.ObservableName;
            notificationTransactionDataBE.ObservationStatus = notificationTransactionDataIE.ObservationStatus;
            notificationTransactionDataBE.ObservationTime = notificationTransactionDataIE.ObservationTime;
            notificationTransactionDataBE.RemediationPlanId = notificationTransactionDataIE.RemediationPlanId;
            //notificationTransactionDataBE.RemediationPlanName = notificationTransactionDataIE.RemediationPlanName;
            notificationTransactionDataBE.RemediationStatus = notificationTransactionDataIE.RemediationStatus;
            notificationTransactionDataBE.RemediationPlanTime = notificationTransactionDataIE.RemediationPlanTime;
            return notificationTransactionDataBE;
        }

        public static List<BE.NotificationConfigurationDetails> NotificationConfigurationDetails_IEtoBE(List<IE.NotificationConfigurationDetails> notificationConfigurationIE)
        {
            List<BE.NotificationConfigurationDetails> notificationConfigurationListBE = new List<BE.NotificationConfigurationDetails>();
            foreach (IE.NotificationConfigurationDetails notificationConfigObject in notificationConfigurationIE)
            {
                BE.NotificationConfigurationDetails notificationConfigObjectBE = new BE.NotificationConfigurationDetails();
                notificationConfigObjectBE.ReferenceKey = notificationConfigObject.ReferenceKey;
                notificationConfigObjectBE.ReferenceType = notificationConfigObject.ReferenceType;
                notificationConfigObjectBE.ReferenceValue = notificationConfigObject.ReferenceValue;
                notificationConfigurationListBE.Add(notificationConfigObjectBE);
            }

            return notificationConfigurationListBE;
        }

        public static List<BE.RecipientConfigurationDetails> RecipientConfigurationDetails_IEtoBE(List<IE.RecipientConfigurationDetails> recipientConfigurationIE)
        {
            List<BE.RecipientConfigurationDetails> recipientConfigurationListBE = new List<BE.RecipientConfigurationDetails>();
            foreach (IE.RecipientConfigurationDetails recipientConfigObject in recipientConfigurationIE)
            {
                BE.RecipientConfigurationDetails recipientConfigObjectBE = new BE.RecipientConfigurationDetails();
                recipientConfigObjectBE.ReferenceKey = recipientConfigObject.ReferenceKey;
                recipientConfigObjectBE.RecipientName = recipientConfigObject.RecipientName;
                recipientConfigObjectBE.ReferenceType = recipientConfigObject.ReferenceType;
                recipientConfigObjectBE.ReferenceValue = recipientConfigObject.ReferenceValue;
                recipientConfigurationListBE.Add(recipientConfigObjectBE);
            }

            return recipientConfigurationListBE;
        }

        public static BE.NotificationConfigurationDetails AnomalyReason_IEtoBE(IE.NotificationConfigurationDetails notificationConfigurationIE)
        {            
            BE.NotificationConfigurationDetails notificationConfigObjectBE = new BE.NotificationConfigurationDetails();
            notificationConfigObjectBE.ReferenceKey = notificationConfigurationIE.ReferenceKey;
            notificationConfigObjectBE.ReferenceType = notificationConfigurationIE.ReferenceType;
            notificationConfigObjectBE.ReferenceValue = notificationConfigurationIE.ReferenceValue;               

            return notificationConfigObjectBE;
        }
    }
}
