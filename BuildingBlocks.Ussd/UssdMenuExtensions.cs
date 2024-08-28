namespace BuildingBlocks.Ussd
{
    public static class UssdMenuExtensions
    {
        public static UssdMenu WithHeader(this UssdMenu menu, string header)
        {
            menu.Header = header;
            return menu;
        }

        public static UssdMenu WithFooter(this UssdMenu menu, string footer)
        {
            menu.Footer = footer;
            return menu;
        }
    }
}
