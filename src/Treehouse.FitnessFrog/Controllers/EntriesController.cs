using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        //Add Entry View
        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date = DateTime.Today,


            };
            //TODO Populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();

            return View(entry);
        }


        //Post Action Method
        [HttpPost]
        public ActionResult Add(Entry entry)
        {
            ValidateEntry(entry);
            //Extract form field data from AddEntry Page
            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);

                return RedirectToAction("Index");
            }
            SetupActivitiesSelectListItems();

            return View(entry);
        }


        //int? to allow nullable peram
        public ActionResult Edit(int? id)
        { //nullable id perameter
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //TODO get requested entry from the repository
            //make call to repository "GetEntry()" method
            //ID param is nullable so needs to be casted to int

            Entry entry = _entriesRepository.GetEntry((int)id);
            //TODO Return the status of "not found" if the entry was not found 
            if(entry == null)
            {
                return HttpNotFound();
            }
            //TODO Populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();
            //TODO pass the entry into the view
            return View(entry);
        }


        [HttpPost]
        public ActionResult Edit(Entry entry)
        {

            //TODO validate entry
            ValidateEntry(entry);
            //TODO If the entry is valid...
            //1) Use repository to update the entry
            //2) Redirect the user to the "Entries" list page
            if(ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }

            //TODO Populate the activities select list items ViewBag property
            SetupActivitiesSelectListItems();

            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View();
        }
        private void ValidateEntry(Entry entry)
        {
            //if there aren't any "Duration" field validation errors
            //then make sure that the duration is greater than "0"
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration field value must be greater than '0'");
            }
        }
        private void SetupActivitiesSelectListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(
                            Data.Data.Activities, "Id", "Name");
        }
    }
}