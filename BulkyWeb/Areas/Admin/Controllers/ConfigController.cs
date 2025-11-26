using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController : ControllerBase
    {
        /// <summary>
        /// Get Ancestral configuration
        /// </summary>
        [HttpGet("ancestral")]
        public IActionResult GetAncestralConfig()
        {
            var config = new
            {
                side = "左側",
                section = "甲區",
                level = "1層",
                position = "000"
            };
            return Ok(config);
        }

        /// <summary>
        /// Get Kindness configuration
        /// </summary>
        [HttpGet("kindness")]
        public IActionResult GetKindnessConfig()
        {
            var config = new
            {
                side = "樓上",
                section = "上廳",
                level = "1層",
                position = "000"
            };
            return Ok(config);
        }

        /// <summary>
        /// Get app settings
        /// </summary>
        [HttpGet("app-settings")]
        public IActionResult GetAppSettings()
        {
            var settings = new
            {
                appName = "BulkyBook Position Management",
                version = "1.0",
                environment = "development"
            };
            return Ok(settings);
        }
    }
}
