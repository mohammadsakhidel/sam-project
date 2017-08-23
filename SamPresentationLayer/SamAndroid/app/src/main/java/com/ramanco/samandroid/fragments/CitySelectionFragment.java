package com.ramanco.samandroid.fragments;


import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.text.Editable;
import android.text.TextWatcher;
import android.util.Pair;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ListView;
import android.widget.Toast;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.adapters.PairAdapter;
import com.ramanco.samandroid.code.objects.KeyValuePair;
import com.ramanco.samandroid.code.utils.CityUtil;
import com.ramanco.samandroid.code.utils.ExceptionManager;
import com.ramanco.samandroid.code.utils.PrefUtil;
import com.rey.material.widget.EditText;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.lang.reflect.Array;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.StringTokenizer;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

public class CitySelectionFragment extends Fragment {

    //region Fields:
    List<KeyValuePair> allCities;
    //endregion

    //region Ctors:
    public CitySelectionFragment() {
    }
    //endregion

    //region Overrides:
    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
        View fragmentView = inflater.inflate(R.layout.fragment_city_selection, container, false);

        try {
            loadCities(fragmentView);

            //region edit text on change:
            EditText etCity = (EditText) fragmentView.findViewById(R.id.et_city);
            etCity.addTextChangedListener(new TextWatcher() {
                @Override
                public void beforeTextChanged(CharSequence s, int start, int count, int after) {

                }

                @Override
                public void onTextChanged(CharSequence s, int start, int before, int count) {

                }

                @Override
                public void afterTextChanged(Editable s) {
                    try {
                        filterCities(s.toString());
                    } catch (Exception ex) {
                        ExceptionManager.Handle(getActivity(), ex);
                    }
                }
            });
            //endregion
        } catch (Exception ex) {
            ExceptionManager.Handle(getActivity(), ex);
        }

        return fragmentView;
    }
    //endregion

    //region Methods:
    private void loadCities(View view) throws ParserConfigurationException, IOException, SAXException {
        allCities = CityUtil.getAllCities(getActivity());
        PairAdapter adapter = new PairAdapter(getActivity(), allCities.toArray(new KeyValuePair[allCities.size()]));
        ListView lvCities = (ListView) view.findViewById(R.id.lv_cities);
        lvCities.setAdapter(adapter);
        lvCities.setOnItemClickListener(new AdapterView.OnItemClickListener() {
            @Override
            public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                try {
                    KeyValuePair pair = (KeyValuePair) parent.getItemAtPosition(position);
                    PrefUtil.setCityID(getActivity(), Integer.parseInt(pair.getKey()));
                    if (onCitySelectRunnable != null) {
                        onCitySelectRunnable.run();
                    }
                } catch (Exception ex) {
                    ExceptionManager.Handle(getActivity(), ex);
                }
            }
        });
    }

    private void filterCities(String text) {
        View fragmentView = getView();
        if (fragmentView != null) {
            // filter cities:
            List<KeyValuePair> filteredCities = new ArrayList<>();
            for (KeyValuePair city : allCities) {
                if (city.getValue().contains(text) || text.contains(city.getValue())) {
                    filteredCities.add(city);
                }
            }
            // load list view:
            PairAdapter adapter = new PairAdapter(getActivity(), filteredCities.toArray(new KeyValuePair[filteredCities.size()]));
            ListView lvCities = (ListView) fragmentView.findViewById(R.id.lv_cities);
            lvCities.setAdapter(adapter);

        }
    }
    //endregion

    //region Fields:
    Runnable onCitySelectRunnable;
    //endregion

    //region Getter & Setters:
    public Runnable getOnCitySelectRunnable() {
        return onCitySelectRunnable;
    }

    public void setOnCitySelectRunnable(Runnable onCitySelectRunnable) {
        this.onCitySelectRunnable = onCitySelectRunnable;
    }
    //endregion

}