using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Controllers
{
    public class StudentsController : Controller
    {
        private readonly SchoolContext _context;

        public StudentsController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Students
        // Adding in ability to have the user sort the data via column name headings.  It will get the sortOrder value from the query string
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            // Contains the current sort order, so when you change to a new page it keeps that value
            ViewData["CurrentSort"] = sortOrder;

            // These will be used by the view to configure the column heading links.  Will also need to modify Students/index.cshtml
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            // Used for the search box.  Make sure it is added to the correct form
            ViewData["CurrentFilter"] = searchString;

            // If the search string changes, reset the page back to 1 since the number of results probably changed.  This works because if you click "Submit", searchString is no long null
            if (searchString != null)
            {
                pageNumber = 1;
            } else
            {
                searchString = currentFilter;
            }

            // Create your query
            var students = from s in _context.Students
                           select s;

            //  Now check if the search string is empty.  If not, search for the string in both name columns
            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.LastName.Contains(searchString)
               || s.FirstMidName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            // Change this to change how many results display per page
            int pageSize = 6;

            // Since we don't care about keeping it 100% up to date, use AsNoTracking for speed improvement
            // Original, without pagination on the results 
            // return View(await students.AsNoTracking().ToListAsync());

            // Take the student query into a single page of students in a collection. Then the page is pages to the view
            // pageNumber ?? 1, means it will return either the value of pageNumber, or 1 if pageNumber is null
            return View(await PaginatedList<Student>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var student = await _context.Students
                // Include the Student.Enrollments navigation property
                .Include(s => s.Enrollments)
                // Now include the Enrollment.Course navigation Property
                .ThenInclude(e => e.Course)
                // If we aren't updating anything, this will improve performance
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                // Takes the data submitted by the form and tries to save it to the database
                if (ModelState.IsValid)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            // If there is an error, return a message to the user
            catch (DbUpdateException /* ex */)
            {
                ModelState.AddModelError("", "Unable to save changes.  Try again, and if the the problem persists, please see your system administrator");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            if (id == null)
            {
                return NotFound();
            }
            var studentToUpdate = await _context.Students.FirstOrDefaultAsync(s => s.ID == id);
            if (await TryUpdateModelAsync<Student>(
                studentToUpdate,
                "",
                s => s.FirstMidName, s => s.LastName, s => s.EnrollmentDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException /* ex */)
                {
                    //Log the error (uncomment ex variable name and write a log.)
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: Students/Delete/5
        // Adding try/catch block to handle any errors
        public async Task<IActionResult> Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);
            if (student == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed.  Try again, and if the the problem persists, see your systems adminitrator.";
            }

            return View(student);
        }

        // POST: Students/Delete/5
        // This does the actual delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // First get the entity
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Call the removal method to set the status of the entity to Deleted
                _context.Students.Remove(student);

                // Now a SQL Delete statement is generated and the entity is removed
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            } catch (DbUpdateException /* ex */)
            {
                // Log the error
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesErrpr = true });
            }

        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.ID == id);
        }
    }
}
