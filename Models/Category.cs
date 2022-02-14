namespace OnlineShop.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class GetNameById
    {
        private readonly ApplicationContext _context;
        public GetNameById(ApplicationContext context)
        {
            _context = context;
        }
        public string Execute(int id)
        {
            var category = _context.Categories.Find(id);
            if(category != null)
            {
                return category.Name;
            }
            return "";
        }
    }
}
