using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication_InformationSecurityRiskAssessmentSystem.Data;
using WebApplication_InformationSecurityRiskAssessmentSystem.Models;


namespace WebApplication_InformationSecurityRiskAssessmentSystem.Controllers
{
    public class ContactsController : Controller

    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        //1

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdmContacts(string? searchString)

        {
            var contacts = from c in _context.Contacts
                           select c;

            if (!string.IsNullOrEmpty(searchString))
            {
                contacts = contacts.Where(c => c.Name.StartsWith(searchString));
            }
            return View(await contacts.ToListAsync());
        }

        //2
        [HttpGet]
        public IActionResult Add()
        {

            ViewBag.Action = "Add";
            return View("Add", new Contact());
        }
        [Authorize(Roles = "Expert")]

        [HttpPost]
        public IActionResult Add(Contact contact)
        {
            if (ModelState.IsValid)
            {

                contact.DateAdded = DateTime.Now;
                _context.Contacts.Add(contact);

                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(contact);
        }



        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AdmContacts));
        }
        //4
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

    }
}
