﻿using System;
using System.Reflection;

using UnityEngine;

using ColossalFramework;
using ColossalFramework.UI;

namespace FineRoadTool
{
    class UIToolOptionsButton : UICheckBox
    {
        private UIButton m_button;
        private UIPanel m_toolOptionsPanel;
        private UISlider m_elevationStepSlider;
        private UILabel m_elevationStepLabel;

        private UICheckBox m_normalModeButton;
        private UICheckBox m_groundModeButton;
        private UICheckBox m_elevatedModeButton;
        private UICheckBox m_bridgeModeButton;

        private UICheckBox m_straightSlope;

        private UITextureAtlas m_atlas;

        private UIComponent m_parent;

        public override void Start()
        {
            LoadResources();

            CreateButton();
            CreateOptionPanel();
        }

        public override void Update()
        {
            if(parent != m_parent)
            {
                m_parent = parent;
                if (parent is UIMultiStateButton)
                {
                    relativePosition = Vector2.zero;
                    parent.BringToFront();
                }
                else
                    relativePosition = new Vector2(36, 0);

                if (m_toolOptionsPanel != null)
                    m_toolOptionsPanel.isVisible = isVisible && isChecked;

                UpdateInfo();
            }
        }

        public void UpdateInfo()
        {
            if (FineRoadTool.instance == null) return;

            m_button.text = m_elevationStepLabel.text = FineRoadTool.instance.elevationStep + "m\n";
            m_elevationStepSlider.value = FineRoadTool.instance.elevationStep;
            m_straightSlope.isChecked = FineRoadTool.instance.straightSlope;

            m_button.normalFgSprite = FineRoadTool.instance.straightSlope ? "ToolbarIconGroup1Hovered" : null;

            switch (FineRoadTool.instance.mode)
            {
                case FineRoadTool.Mode.Normal:
                    m_button.text += "Nrm\n";
                    m_normalModeButton.SimulateClick();
                    break;
                case FineRoadTool.Mode.Ground:
                    m_button.text += "Gnd\n";
                    m_groundModeButton.SimulateClick();
                    break;
                case FineRoadTool.Mode.Elevated:
                    m_button.text += "Elv\n";
                    m_elevatedModeButton.SimulateClick();
                    break;
                case FineRoadTool.Mode.Bridge:
                    m_button.text += "Bdg\n";
                    m_bridgeModeButton.SimulateClick();
                    break;
            }

            m_button.text += FineRoadTool.instance.elevation + "m";
        }

        private void CreateButton()
        {
            m_button = AddUIComponent<UIButton>();
            m_button.size = new Vector2(36, 36);
            m_button.textScale = 0.7f;
            m_button.playAudioEvents = true;
            m_button.relativePosition = Vector2.zero;

            m_button.tooltip = "Fine Road Tool " + ModInfo.version + "\n\nClick here for Tool Options";

            m_button.textColor = Color.white;
            m_button.textScale = 0.7f;
            m_button.dropShadowOffset = new Vector2(2, -2);
            m_button.useDropShadow = true;

            m_button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            m_button.wordWrap = true;

            m_button.normalBgSprite = "OptionBase";
            m_button.hoveredBgSprite = "OptionBaseHovered";
            m_button.pressedBgSprite = "OptionBasePressed";
            m_button.disabledBgSprite = "OptionBaseDisabled";

            m_button.eventClick += (c, p) => { base.OnClick(p); };

            eventCheckChanged += (c, isChecked) =>
            {
                m_toolOptionsPanel.isVisible = isVisible && isChecked;
                if (isChecked)
                {
                    if(m_toolOptionsPanel.absolutePosition.x <0)
                        m_toolOptionsPanel.absolutePosition = new Vector2(absolutePosition.x - 82, absolutePosition.y - 180);

                    m_toolOptionsPanel.absolutePosition = new Vector2(
                        Mathf.Clamp(m_toolOptionsPanel.absolutePosition.x, 0, Screen.width - m_toolOptionsPanel.width),
                        Mathf.Clamp(m_toolOptionsPanel.absolutePosition.y, 0, Screen.height - m_toolOptionsPanel.height));

                    m_button.normalBgSprite = "OptionBaseFocused";

                    m_toolOptionsPanel.BringToFront();
                }
                else
                {
                    m_button.normalBgSprite = "OptionBase";
                }
                UpdateInfo();
            };
        }

        protected override void OnVisibilityChanged()
        {
            if(m_toolOptionsPanel != null)
                m_toolOptionsPanel.isVisible = isVisible && isChecked;
        }

        protected override void OnClick(UIMouseEventParameter p) { }

        private void CreateOptionPanel()
        {
            m_toolOptionsPanel = UIView.GetAView().AddUIComponent(typeof(UIPanel)) as UIPanel;
            m_toolOptionsPanel.atlas = ResourceLoader.GetAtlas("Ingame");
            m_toolOptionsPanel.backgroundSprite = "SubcategoriesPanel";
            m_toolOptionsPanel.size = new Vector2(200, 180);
            m_toolOptionsPanel.absolutePosition = new Vector2(-1000, -1000);

            m_toolOptionsPanel.isVisible = false;

            UIDragHandle dragHandle = m_toolOptionsPanel.AddUIComponent<UIDragHandle>();
            dragHandle.size = m_toolOptionsPanel.size;
            dragHandle.relativePosition = Vector3.zero;
            dragHandle.target = parent;

            // Elevation step
            UILabel label = m_toolOptionsPanel.AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = "Elevation Step:";
            label.relativePosition = new Vector2(8, 8);
            label.SendToBack();

            UIPanel sliderPanel = m_toolOptionsPanel.AddUIComponent<UIPanel>();
            sliderPanel.atlas = m_toolOptionsPanel.atlas;
            sliderPanel.backgroundSprite = "GenericPanel";
            sliderPanel.color = new Color32(206, 206, 206, 255);
            sliderPanel.size = new Vector2(m_toolOptionsPanel.width - 16, 36);
            sliderPanel.relativePosition = new Vector2(8, 28);

            m_elevationStepLabel = sliderPanel.AddUIComponent<UILabel>();
            m_elevationStepLabel.atlas = m_toolOptionsPanel.atlas;
            m_elevationStepLabel.backgroundSprite = "TextFieldPanel";
            m_elevationStepLabel.verticalAlignment = UIVerticalAlignment.Bottom;
            m_elevationStepLabel.textAlignment = UIHorizontalAlignment.Center;
            m_elevationStepLabel.textScale = 0.7f;
            m_elevationStepLabel.text = "3m";
            m_elevationStepLabel.autoSize = false;
            m_elevationStepLabel.color = new Color32(91, 97, 106, 255);
            m_elevationStepLabel.size = new Vector2(38, 15);
            m_elevationStepLabel.relativePosition = new Vector2(sliderPanel.width - m_elevationStepLabel.width - 8, 10);

            m_elevationStepSlider = sliderPanel.AddUIComponent<UISlider>();
            m_elevationStepSlider.size = new Vector2(sliderPanel.width - 20 - m_elevationStepLabel.width - 8, 18);
            m_elevationStepSlider.relativePosition = new Vector2(10, 10);

            m_elevationStepSlider.tooltip = OptionsKeymapping.elevationStepUp.ToLocalizedString("KEYNAME") + " and " + OptionsKeymapping.elevationStepDown.ToLocalizedString("KEYNAME") + " to change Elevation Step";

            UISlicedSprite bgSlider = m_elevationStepSlider.AddUIComponent<UISlicedSprite>();
            bgSlider.atlas = m_toolOptionsPanel.atlas;
            bgSlider.spriteName = "BudgetSlider";
            bgSlider.size = new Vector2(m_elevationStepSlider.width, 9);
            bgSlider.relativePosition = new Vector2(0, 4);

            UISlicedSprite thumb = m_elevationStepSlider.AddUIComponent<UISlicedSprite>();
            thumb.atlas = m_toolOptionsPanel.atlas;
            thumb.spriteName = "SliderBudget";
            m_elevationStepSlider.thumbObject = thumb;

            m_elevationStepSlider.stepSize = 1;
            m_elevationStepSlider.minValue = 1;
            m_elevationStepSlider.maxValue = 12;
            m_elevationStepSlider.value = 3;

            m_elevationStepSlider.eventValueChanged += (c, v) =>
            {
                if(v != FineRoadTool.instance.elevationStep)
                {
                    FineRoadTool.instance.elevationStep = (int)v;
                    UpdateInfo();
                }
            };

            // Modes
            label = m_toolOptionsPanel.AddUIComponent<UILabel>();
            label.textScale = 0.9f;
            label.text = "Modes:";
            label.relativePosition = new Vector2(8, 72);
            label.SendToBack();

            UIPanel modePanel = m_toolOptionsPanel.AddUIComponent<UIPanel>();
            modePanel.atlas = m_toolOptionsPanel.atlas;
            modePanel.backgroundSprite = "GenericPanel";
            modePanel.color = new Color32(206, 206, 206, 255);
            modePanel.size = new Vector2(m_toolOptionsPanel.width - 16, 52);
            modePanel.relativePosition = new Vector2(8, 92);

            m_normalModeButton = CreateModeCheckBox(modePanel, "NormalMode", "Unmodded road placement behavior");
            m_normalModeButton.relativePosition = new Vector2(8, 8);

            m_groundModeButton = CreateModeCheckBox(modePanel, "GroundMode", "Forces the ground to follow the elevation of the road");
            m_groundModeButton.relativePosition = new Vector2(52, 8);

            m_elevatedModeButton = CreateModeCheckBox(modePanel, "ElevatedMode", "Forces the use of elevated pieces if available");
            m_elevatedModeButton.relativePosition = new Vector2(96, 8);

            m_bridgeModeButton = CreateModeCheckBox(modePanel, "BridgeMode", "Forces the use of bridge pieces if available");
            m_bridgeModeButton.relativePosition = new Vector2(140, 8);

            m_normalModeButton.isChecked = true;

            // Straight Slope
            m_straightSlope = CreateCheckBox(m_toolOptionsPanel);
            m_straightSlope.label.text = "Straight slope";
            m_straightSlope.tooltip = "Makes the road go straight from A to B instead of following the terrain";
            m_straightSlope.isChecked = false;
            m_straightSlope.relativePosition = new Vector3(8, 152);

            m_straightSlope.eventCheckChanged += (c, state) =>
            {
                FineRoadTool.instance.straightSlope = state;
            };
        }

        private UICheckBox CreateCheckBox(UIComponent parent)
        {
            UICheckBox checkBox = (UICheckBox)parent.AddUIComponent<UICheckBox>();

            checkBox.width = 300f;
            checkBox.height = 20f;
            checkBox.clipChildren = true;

            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.atlas = m_toolOptionsPanel.atlas;
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = Vector3.zero;

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).atlas = m_toolOptionsPanel.atlas;
            ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.label.text = " ";
            checkBox.label.textScale = 0.9f;
            checkBox.label.relativePosition = new Vector3(22f, 2f);

            return checkBox;
        }

        private UICheckBox CreateModeCheckBox(UIComponent parent, string spriteName, string toolTip)
        {
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.size = new Vector2(36, 36);
            checkBox.group = parent;

            UIButton button = checkBox.AddUIComponent<UIButton>();
            button.atlas = m_atlas;
            button.relativePosition = new Vector2(0, 0);

            button.tooltip = toolTip + "\n\n" + OptionsKeymapping.modesCycleLeft.ToLocalizedString("KEYNAME") + " and " + OptionsKeymapping.modesCycleRight.ToLocalizedString("KEYNAME") + " to cycle Modes";

            button.normalBgSprite = "OptionBase";
            button.hoveredBgSprite = "OptionBaseHovered";
            button.pressedBgSprite = "OptionBasePressed";
            button.disabledBgSprite = "OptionBaseDisabled";

            button.normalFgSprite = spriteName;
            button.hoveredFgSprite = spriteName + "Hovered";
            button.pressedFgSprite = spriteName + "Pressed";
            button.disabledFgSprite = spriteName + "Disabled";

            checkBox.eventCheckChanged += (c, s) =>
            {
                if (s)
                {
                    button.normalBgSprite = "OptionBaseFocused";
                    button.normalFgSprite = spriteName + "Focused";
                }
                else
                {
                    button.normalBgSprite = "OptionBase";
                    button.normalFgSprite = spriteName;
                }

                UpdateMode();
            };

            return checkBox;
        }

        private void UpdateMode()
        {
            if (m_normalModeButton.isChecked) FineRoadTool.instance.mode = FineRoadTool.Mode.Normal;
            if (m_groundModeButton.isChecked) FineRoadTool.instance.mode = FineRoadTool.Mode.Ground;
            if (m_elevatedModeButton.isChecked) FineRoadTool.instance.mode = FineRoadTool.Mode.Elevated;
            if (m_bridgeModeButton.isChecked) FineRoadTool.instance.mode = FineRoadTool.Mode.Bridge;
        }

        private void LoadResources()
        {
            string[] spriteNames = new string[]
			{
				"NormalMode",
				"NormalModeDisabled",
				"NormalModeFocused",
				"NormalModeHovered",
				"NormalModePressed",
				"BridgeMode",
				"BridgeModeDisabled",
				"BridgeModeFocused",
				"BridgeModeHovered",
				"BridgeModePressed",
				"ElevatedMode",
				"ElevatedModeDisabled",
				"ElevatedModeFocused",
				"ElevatedModeHovered",
				"ElevatedModePressed",
				"GroundMode",
				"GroundModeDisabled",
				"GroundModeFocused",
				"GroundModeHovered",
				"GroundModePressed"
			};

            m_atlas = ResourceLoader.CreateTextureAtlas("FineRoadTool", spriteNames, "FineRoadTool.Icons.");

            UITextureAtlas defaultAtlas = ResourceLoader.GetAtlas("Ingame");
            Texture2D[] textures = new Texture2D[]
            {
                defaultAtlas["OptionBase"].texture,
                defaultAtlas["OptionBaseFocused"].texture,
                defaultAtlas["OptionBaseHovered"].texture,
                defaultAtlas["OptionBasePressed"].texture,
                defaultAtlas["OptionBaseDisabled"].texture
            };

            ResourceLoader.AddTexturesInAtlas(m_atlas, textures);
        }
    }
}