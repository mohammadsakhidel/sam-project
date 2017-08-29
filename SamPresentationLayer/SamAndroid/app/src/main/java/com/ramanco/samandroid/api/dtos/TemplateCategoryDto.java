package com.ramanco.samandroid.api.dtos;

import com.google.gson.annotations.Expose;
import com.google.gson.annotations.SerializedName;

public class TemplateCategoryDto {

    @SerializedName("ID")
    @Expose
    int id;

    @SerializedName("Name")
    @Expose
    String name;

    @SerializedName("Order")
    @Expose
    int order;

    @SerializedName("Description")
    @Expose
    String description;

    //region Getters & Setters:

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getOrder() {
        return order;
    }

    public void setOrder(int order) {
        this.order = order;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }

    //endregion

}
