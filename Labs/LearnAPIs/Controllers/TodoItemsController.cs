using LearnAPIs.Data;
using LearnAPIs.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPIs.Controllers
{

    [ApiController]
    [Route("api/[controller]")]

    // This attribute applies the default API conventions to the controller,
    // which helps with generating consistent API documentation and behavior.
    [ApiConventionType(typeof(DefaultApiConventions))] 

    public class TodoItemsController : ControllerBase
    {

        private readonly TodoDbContext _context;

        public TodoItemsController(TodoDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public List<TodoItem> GetItems()
        {
            return _context.TodoItems.ToList();
        }


        // api/todoitems/2
        [HttpGet("{id}")]
        public ActionResult<TodoItem> GetById(int id)
        {

            var item = _context.TodoItems.FirstOrDefault(i => i.Id == id);

            if (item == null)
                return NotFound();

            return item;

        }


        [HttpPost]
        public ActionResult<TodoItem> CreateItem(TodoItem item)
        {
            _context.TodoItems.Add(item);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        } 
        

        [HttpPut("{id}")]
        public ActionResult UpdateItem(int id, TodoItem updatedItem)
        {
            var Item = _context.TodoItems.FirstOrDefault(i => i.Id == id);

            if (Item == null)
                return NotFound();

            foreach(var i in _context.TodoItems)
            {
                if(i.Id == id)
                {
                    Item.Name = updatedItem.Name;
                }
            }

            _context.SaveChanges();
            return Ok(_context.TodoItems);
        }


        [HttpPut("MarkComplete/{id}")]
        public ActionResult MarkComplete(int id)
        {
            var Item = _context.TodoItems.FirstOrDefault(i => i.Id == id);

            if (Item == null)
                return NotFound();

            foreach (var i in _context.TodoItems)
            {
                if (i.Id == id)
                {
                    Item.IsComplete = true;
                }
            }

            _context.SaveChanges();
            return Ok(Item);
        }


        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var Item = _context.TodoItems.FirstOrDefault(i => i.Id == id);

            if (Item == null)
                return NotFound();

            _context.TodoItems.Remove(Item);
            _context.SaveChanges();

            return Ok(_context.TodoItems);
        }

    }
}
