﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Stokvel_Groups_Home_RSA.common.Alert.TempData
{
    public static class AlertExtensions
    {
        private const string AlertKey = "SimpleToDo.Alert";

        public static void AddAlertSuccess(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new Alert(message, "alert-success"));

            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        public static void AddAlertInfo(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new Alert(message, "alert-info"));

            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        public static void AddAlertWarning(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new Alert(message, "alert-warning"));

            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        public static void AddAlertDanger(this Controller controller, string message)
        {
            var alerts = GetAlerts(controller);

            alerts.Add(new Alert(message, "alert-danger"));

            controller.TempData[AlertKey] = JsonConvert.SerializeObject(alerts);
        }

        private static ICollection<Alert> GetAlerts(Controller controller)
        {
            if (controller.TempData[AlertKey] == null)
            {
                controller.TempData[AlertKey] = JsonConvert.SerializeObject(new HashSet<Alert>());
            }

            var alerts = JsonConvert.DeserializeObject<ICollection<Alert>>(controller.TempData[AlertKey]?.ToString());

            return alerts ?? new HashSet<Alert>();
        }

    }

}