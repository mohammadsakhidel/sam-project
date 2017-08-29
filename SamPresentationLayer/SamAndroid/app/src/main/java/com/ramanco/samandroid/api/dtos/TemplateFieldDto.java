package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

class TemplateFieldDto {

    @SerializedName("ID")
    @Expose
    int id;

    @SerializedName("TemplateID")
    @Expose
    int templateID;

    @SerializedName("Name")
    @Expose
    String name;

    @SerializedName("DisplayName")
    @Expose
    String displayName;

    @SerializedName("Description")
    @Expose
    String description;

    @SerializedName("X")
    @Expose
    double x;

    @SerializedName("Y")
    @Expose
    double y;

    @SerializedName("FontFamily")
    @Expose
    String fontFamily;

    @SerializedName("FontSize")
    @Expose
    String fontSize;

    @SerializedName("Bold")
    @Expose
    boolean bold;

    @SerializedName("TextColor")
    @Expose
    String textColor;

    @SerializedName("FlowDirection")
    @Expose
    String flowDirection;

    @SerializedName("BoxWidth")
    @Expose
    double boxWidth;

    @SerializedName("BoxHeight")
    @Expose
    double boxHeight;

    @SerializedName("HorizontalContentAlignment")
    @Expose
    String horizontalContentAlignment;

    @SerializedName("VerticalContentAlignment")
    @Expose
    String verticalContentAlignment;

    @SerializedName("WrapContent")
    @Expose
    boolean wrapContent;

    //region Getters & Setters:

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getTemplateID() {
        return templateID;
    }

    public void setTemplateID(int templateID) {
        this.templateID = templateID;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getDisplayName() {
        return displayName;
    }

    public void setDisplayName(String displayName) {
        this.displayName = displayName;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    public double getX() {
        return x;
    }

    public void setX(double x) {
        this.x = x;
    }

    public double getY() {
        return y;
    }

    public void setY(double y) {
        this.y = y;
    }

    public String getFontFamily() {
        return fontFamily;
    }

    public void setFontFamily(String fontFamily) {
        this.fontFamily = fontFamily;
    }

    public String getFontSize() {
        return fontSize;
    }

    public void setFontSize(String fontSize) {
        this.fontSize = fontSize;
    }

    public boolean isBold() {
        return bold;
    }

    public void setBold(boolean bold) {
        this.bold = bold;
    }

    public String getTextColor() {
        return textColor;
    }

    public void setTextColor(String textColor) {
        this.textColor = textColor;
    }

    public String getFlowDirection() {
        return flowDirection;
    }

    public void setFlowDirection(String flowDirection) {
        this.flowDirection = flowDirection;
    }

    public double getBoxWidth() {
        return boxWidth;
    }

    public void setBoxWidth(double boxWidth) {
        this.boxWidth = boxWidth;
    }

    public double getBoxHeight() {
        return boxHeight;
    }

    public void setBoxHeight(double boxHeight) {
        this.boxHeight = boxHeight;
    }

    public String getHorizontalContentAlignment() {
        return horizontalContentAlignment;
    }

    public void setHorizontalContentAlignment(String horizontalContentAlignment) {
        this.horizontalContentAlignment = horizontalContentAlignment;
    }

    public String getVerticalContentAlignment() {
        return verticalContentAlignment;
    }

    public void setVerticalContentAlignment(String verticalContentAlignment) {
        this.verticalContentAlignment = verticalContentAlignment;
    }

    public boolean isWrapContent() {
        return wrapContent;
    }

    public void setWrapContent(boolean wrapContent) {
        this.wrapContent = wrapContent;
    }

    //endregion

}
