using System.Collections.Generic;
using Heroes.Game.Core.Events;
using Heroes.Game.Core.Events.Bus;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace Heroes.Presentation.UI.BuildMenu
{
    public class BuildMenuPresenter : MonoBehaviour
    {
        [SerializeField] private UIDocument document;
        [SerializeField] private VisualTreeAsset viewAsset;
        [SerializeField] private StyleSheet styleSheet;

        private BuildMenuViewModel _viewModel;
        private IGameEventBus _eventBus;
        private readonly List<BuildMenuItemVm> _items = new();

        private ListView _listView;
        private Label _goldLabel;

        [Inject]
        public void Construct(BuildMenuViewModel viewModel, IGameEventBus eventBus)
        {
            _viewModel = viewModel;
            _eventBus = eventBus;
        }

        private void Awake()
        {
            if (document == null)
            {
                document = GetComponent<UIDocument>();
            }

            if (document == null)
            {
                return;
            }

            var root = document.rootVisualElement;
            if (viewAsset != null)
            {
                root.Clear();
                viewAsset.CloneTree(root);
            }

            if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
            {
                root.styleSheets.Add(styleSheet);
            }

            _listView = root.Q<ListView>("build-menu-list");
            _goldLabel = root.Q<Label>("build-menu-gold");

            if (_listView != null)
            {
                _listView.itemsSource = _items;
                _listView.selectionType = SelectionType.Single;
                _listView.fixedItemHeight = 120f;
                _listView.makeItem = CreateItem;
                _listView.bindItem = BindItem;
                _listView.selectionChanged += OnSelectionChanged;
            }
        }

        private void OnEnable()
        {
            if (_eventBus != null)
            {
                _eventBus.Subscribe<ResourcesChangedEvent>(OnResourcesChanged);
                _eventBus.Subscribe<BuildingSelectionChangedEvent>(OnSelectionChanged);
            }

            Refresh();
        }

        private void OnDisable()
        {
            if (_eventBus != null)
            {
                _eventBus.Unsubscribe<ResourcesChangedEvent>(OnResourcesChanged);
                _eventBus.Unsubscribe<BuildingSelectionChangedEvent>(OnSelectionChanged);
            }
        }

        public IReadOnlyList<BuildMenuItemVm> GetItems()
        {
            return _viewModel.GetItems();
        }

        public int GetGold()
        {
            return _viewModel.GetGold();
        }

        public void SelectBuilding(string id)
        {
            _viewModel.SelectBuilding(id);
        }

        private VisualElement CreateItem()
        {
            var root = new VisualElement();
            root.AddToClassList("build-menu__item");

            var frame = new VisualElement();
            frame.AddToClassList("build-menu__item-frame");

            var icon = new VisualElement();
            icon.AddToClassList("build-menu__item-icon");

            frame.Add(icon);
            root.Add(frame);

            var meta = new VisualElement();
            meta.AddToClassList("build-menu__item-meta");

            var name = new Label();
            name.name = "build-menu-item-name";
            name.AddToClassList("build-menu__item-name");

            var cost = new Label();
            cost.name = "build-menu-item-cost";
            cost.AddToClassList("build-menu__item-cost");

            meta.Add(name);
            meta.Add(cost);
            root.Add(meta);

            return root;
        }

        private void BindItem(VisualElement element, int index)
        {
            if (index < 0 || index >= _items.Count)
            {
                return;
            }

            var data = _items[index];
            element.Q<Label>("build-menu-item-name").text = data.Name;
            element.Q<Label>("build-menu-item-cost").text = data.Cost.ToString();

            element.EnableInClassList("build-menu__item--selected", data.Selected);
            element.EnableInClassList("build-menu__item--disabled", !data.Available);
        }

        private void OnSelectionChanged(IEnumerable<object> selection)
        {
            foreach (var item in selection)
            {
                if (item is BuildMenuItemVm vm)
                {
                    _viewModel.SelectBuilding(vm.Id);
                    break;
                }
            }

            Refresh();
        }

        private void Refresh()
        {
            if (_viewModel == null)
            {
                return;
            }

            _items.Clear();
            _items.AddRange(_viewModel.GetItems());

            if (_listView != null)
            {
                _listView.Rebuild();
            }

            if (_goldLabel != null)
            {
                _goldLabel.text = _viewModel.GetGold().ToString();
            }
        }

        private void OnResourcesChanged(ResourcesChangedEvent evt)
        {
            Refresh();
        }

        private void OnSelectionChanged(BuildingSelectionChangedEvent evt)
        {
            Refresh();
        }
    }
}
