//using ContactQueueService.Dto;
//using ContactQueueService.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace ContactQueueService.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class ContactsController : ControllerBase
//    {
//        private readonly IContactService _contactService;
//        private readonly ILogger<ContactsController> _logger;

//        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
//        {
//            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

       
//        /// <summary>
//        /// Create a new contact
//        /// </summary>
//        [HttpPost]
//        [ProducesResponseType(StatusCodes.Status201Created)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        public async Task<ActionResult<ContactDto>> CreateContact(ContactDto contactDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            await _contactService.CreateContactAsync(contactDto);
//            return Ok();
//        }

//        /// <summary>
//        /// Update an existing contact
//        /// </summary>
//        [HttpPut("{id}")]
//        [ProducesResponseType(StatusCodes.Status200OK)]
//        [ProducesResponseType(StatusCodes.Status400BadRequest)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> UpdateContact(Guid id, ContactDto contactDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                var updatedContact = await _contactService.UpdateContactAsync(id, contactDto);
//                return Ok(updatedContact);
//            }
//            catch (KeyNotFoundException ex)
//            {
//                _logger.LogWarning(ex.Message);
//                return NotFound();
//            }
//        }

//        /// <summary>
//        /// Delete a contact
//        /// </summary>
//        [HttpDelete("{id}")]
//        [ProducesResponseType(StatusCodes.Status204NoContent)]
//        [ProducesResponseType(StatusCodes.Status404NotFound)]
//        public async Task<IActionResult> DeleteContact(Guid id)
//        {
//            try
//            {
//                await _contactService.DeleteContactAsync(id);
//                return NoContent();
//            }
//            catch (KeyNotFoundException ex)
//            {
//                _logger.LogWarning(ex.Message);
//                return NotFound();
//            }
//        }
//    }
//}