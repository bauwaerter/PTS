using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace Web.Infrastructure
{
    /// <summary>
    /// Api Base Controller Class
    /// </summary>
    public class BaseApiController : ApiController
    {

        /// <summary>
        /// Gets or sets the claims principal.
        /// </summary>
        protected ClaimsPrincipal ClaimsPrincipal { get; set; }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        protected int UserId
        {
            get
            {
                var id = from c in ClaimsPrincipal.Claims
                         where c.Type == ClaimTypes.NameIdentifier
                         select c.Value;
                return int.Parse(id.FirstOrDefault());
            }
        }

        /// <summary>
        /// Gets the user id.
        /// </summary>
        protected string Username
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BaseApiController()
        {
            this.ClaimsPrincipal = User as ClaimsPrincipal;
        }

        /// <summary>
        /// creates an HttpResponseMessage with a response code of 400
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>a new HttpResponseMessage</returns>
        protected HttpResponseMessage BadRequest(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.BadRequest, content);
        }

        /// <summary>
        /// Creates an <see cref="HttpResponseException"/> to be thrown by the api.
        /// </summary>
        /// <param name="reason">Explanation text, also added to the body.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>A new <see cref="HttpResponseException"/></returns>
        private static HttpResponseException CreateHttpResponseException(string reason, HttpStatusCode code)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = code,
                ReasonPhrase = reason,
                Content = new StringContent(reason)
            };
            throw new HttpResponseException(response);
        }

        /// <summary>
        /// Creates the HttpResponseMessage with a response code of 201
        /// and places the content in the content of the body.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>new HttpResponseMessage</returns>
        protected HttpResponseMessage Created(object content)
        {
            return Request.CreateResponse(HttpStatusCode.Created, content);
        }


        /// <summary>
        /// Creates the HttpResponseMessage with a response code of 409
        /// and places the content in the content of the body.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>HttpResponseMessage</returns>
        protected HttpResponseMessage Conflict(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.Conflict, content);
        }

        /// <summary>
        /// Creates Http response message with response code of 403 
        /// and places the reason in the body of the message
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns></returns>
        protected HttpResponseMessage Forbidden(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.Forbidden, content);
        }

        /// <summary>
        /// Creates Http exception with response code of 500 
        /// and places the reason in the body of the message
        /// </summary>
        /// <param name="reason">The explanation text for the client</param>
        /// <returns>a new HttpResponseException</returns>
        protected HttpResponseException GenericException(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// creates an HttpResponseMessage with a response code of 500
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>a new HttpResponseMessage</returns>
        protected HttpResponseMessage InternalServerError(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.InternalServerError, content);
        }

        /// <summary>
        /// creates an HttpResponseMessage with a response code of 200
        /// and places the JSON serialized object in the body.
        /// </summary>
        /// <param name="content">object</param>
        /// <returns>a new HttpResponseMessage</returns>
        protected HttpResponseMessage Ok(object content)
        {
            return Request.CreateResponse(HttpStatusCode.OK, content);
        }

        /// <summary>
        /// creates an HTTP Response message with a response code Not Acceptable
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>
        /// a new HttpResponseMessage
        /// </returns>
        protected HttpResponseMessage NotAcceptable(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.NotAcceptable, content);
        }

        /// <summary>
        /// creates an HTTP Response message with a response code of 404
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>
        /// a new HttpResponseMessage
        /// </returns>
        protected HttpResponseMessage NotFound(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.NotFound, content);
        }

        /// <summary>
        /// creates an HTTP Response message with a response code of 401
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <returns>HttpResponseMessage along with the reason</returns>
        protected HttpResponseMessage Unauthorized(string reason)
        {
            var content = new { Message = reason };
            return Request.CreateResponse(HttpStatusCode.Unauthorized, content);
        }

    } // class

} // namespace