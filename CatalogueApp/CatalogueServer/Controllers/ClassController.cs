using Microsoft.AspNetCore.Mvc;
using CatalogueServer.Repositories;

namespace CatalogueServer.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly ClassRepository _classRepository;

        public ClassController(ClassRepository classRepository)
        {
            _classRepository = classRepository;
        }

        [HttpGet]
        //get all the classes with the teacher id
        public IActionResult GetClasses(int teacherId)
        {
            var classes = _classRepository.GetClassesByTeacherId(teacherId);
            return Ok(classes);
        }


    }
}
