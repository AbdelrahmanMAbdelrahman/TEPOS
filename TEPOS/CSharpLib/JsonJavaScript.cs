using System.Web.Mvc;

namespace ERP.CSharpLib
{
    public class JsonJavaScript
    {
        public ActionResult jsonReturn=null;
        /// <summary>
        /// for json max length return function
        /// </summary>
        public ActionResult JSONResult(JsonResult ret)
        {
            var jsonMaxLengthResult = ret;
            jsonMaxLengthResult.MaxJsonLength = int.MaxValue;
            jsonReturn = jsonMaxLengthResult;
            return null;
        }

    }
}